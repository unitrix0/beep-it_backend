using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utrix.WebLib.Authentication;

namespace BeepBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AuthControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IMapper _mapper;
        private readonly IPermissionsCache _permissionsCache;
        private readonly DateTime _tokenLifeTime;
        private readonly string _tokenSecretKey;

        public AuthController(IAuthRepository authRepo, IMapper mapper, IConfiguration config, IPermissionsCache permissionsCache)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            IConfiguration config1 = config;
            _permissionsCache = permissionsCache;
            _tokenLifeTime = DateTime.Now.AddSeconds(Convert.ToInt32(config1.GetSection("AppSettings:TokenLifeTime").Value));
            _tokenSecretKey = config1.GetSection("AppSettings:Token").Value;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserForRegistrationDto newUser)
        {
            newUser.Username = newUser.Username.ToLower();

            var userToCreate = _mapper.Map<User>(newUser);
            User createdUser = await _authRepo.Register(userToCreate, newUser.Password);

            var userToReturn = _mapper.Map<UserForEditDto>(createdUser);
            return CreatedAtRoute(nameof(UsersController.GetUser), new { controller = "Users", id = userToReturn.Id }, userToReturn);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserForLoginDto user)
        {
            User userFromRepo = await _authRepo.Login(user.Username.ToLower(), user.Password);
            if (userFromRepo == null) return Unauthorized();

            var mappedUser = _mapper.Map<UserForTokenDto>(userFromRepo);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };

            IList<string> roles = await _authRepo.GetUserRoles(userFromRepo);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            Permission permissions = await _authRepo.GetDefaultPermissions(userFromRepo.Id);
            claims.Add(new Claim(BeepClaimTypes.Permissions, permissions.ToBits()));
            claims.Add(new Claim(BeepClaimTypes.PermissionsSerial, permissions.Serial));
            claims.Add(new Claim(BeepClaimTypes.EnvironmentId, permissions.Environment.Id.ToString()));

            _permissionsCache.AddEntry($"{permissions.UserId},{permissions.Environment.Id}", permissions.Serial, _tokenLifeTime);

            return Ok(new
            {
                token = CreateToken(claims.ToArray(), _tokenSecretKey, _tokenLifeTime),
                mappedUser
            });
        }

        [HttpPost("updatepermissions/{userId}")]
        public async Task<IActionResult> UpdatePermissions(int userId, string tokenString, int environmentId)
        {

            ClaimsPrincipal principal = GetPrincipalFromToken(tokenString);
            var newClaims = principal.Claims.Where(c => c.Type != BeepClaimTypes.Permissions || 
                                                        c.Type != BeepClaimTypes.PermissionsSerial ||
                                                        c.Type != BeepClaimTypes.EnvironmentId).ToList();

            Permission permissions = await _authRepo.GetUserPermissions(userId, environmentId);
            newClaims.Add(new Claim(BeepClaimTypes.Permissions, permissions.ToBits()));
            newClaims.Add(new Claim(BeepClaimTypes.PermissionsSerial, AuthRepository.GeneratePermissionSerial()));
            newClaims.Add(new Claim(BeepClaimTypes.EnvironmentId, permissions.Environment.Id.ToString()));

            var mappedUser = _mapper.Map<UserForTokenDto>(permissions.User);
            string newJwtToken = CreateToken(newClaims.ToArray(), _tokenSecretKey, _tokenLifeTime);
            return new ObjectResult(new
            {
                token = newJwtToken,
                mappedUser
            });
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSecretKey)),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}

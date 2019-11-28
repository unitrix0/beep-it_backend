using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utrix.WebLib.Helpers;

namespace BeepBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IMapper _mapper;
        private readonly IPermissionsCache _permissionsCache;
        private readonly string _tokenSecretKey;
        private readonly int _tokenLifeTimeSeconds;

        public AuthController(IAuthRepository authRepo, IMapper mapper, IConfiguration config, IPermissionsCache permissionsCache)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            _permissionsCache = permissionsCache;
            _tokenLifeTimeSeconds = Convert.ToInt32(config.GetSection("AppSettings:TokenLifeTime").Value);
            _tokenSecretKey = config.GetSection("AppSettings:Token").Value;
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
            Console.WriteLine($"New Login from: " + user);
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

            _permissionsCache.AddEntry(permissions.UserId, permissions.Environment.Id, permissions, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds));

            return Ok(new
            {
                token = JwtHelper.CreateToken(claims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds)),
                mappedUser
            });
        }

        [HttpPost("updatepermissions/{userId}")]
        public async Task<IActionResult> UpdatePermissions(int userId, int environmentId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            List<Claim> newClaims = User.Claims.Where(c => c.Type != BeepClaimTypes.Permissions &&
                                                        c.Type != BeepClaimTypes.PermissionsSerial &&
                                                        c.Type != BeepClaimTypes.EnvironmentId).ToList();

            Permission permissions = await _authRepo.GetUserPermissions(userId, environmentId);
            newClaims.Add(new Claim(BeepClaimTypes.Permissions, permissions.ToBits()));
            newClaims.Add(new Claim(BeepClaimTypes.PermissionsSerial, SerialGenerator.Generate()));
            newClaims.Add(new Claim(BeepClaimTypes.EnvironmentId, permissions.Environment.Id.ToString()));

            var mappedUser = _mapper.Map<UserForTokenDto>(permissions.User);
            string newJwtToken = JwtHelper.CreateToken(newClaims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds));
            return new ObjectResult(new
            {
                token = newJwtToken,
                mappedUser
            });
        }

        [HttpGet("UserExists/{userId}")]
        public async Task<IActionResult> UserExists(int userId, string username)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            bool exists = await _authRepo.UserExists(username);
            return Ok(exists);
        }

    }
}

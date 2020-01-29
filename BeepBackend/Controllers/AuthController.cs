using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utrix.WebLib;
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
        private readonly IAuthorizationService _authService;
        private readonly string _tokenSecretKey;
        private readonly int _tokenLifeTimeSeconds;

        public AuthController(IAuthRepository authRepo, IMapper mapper, IConfiguration config, IPermissionsCache permissionsCache, IAuthorizationService authService)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            _permissionsCache = permissionsCache;
            _authService = authService;
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
            Response.ExposeHeader("location");
            return CreatedAtRoute(nameof(UsersController.GetUser), new { controller = "Users", id = userToReturn.Id }, userToReturn);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserForLoginDto user)
        {
            Console.WriteLine($"New Login from: {user.Username}");
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

            Permission defaultPermission = await _authRepo.GetDefaultPermissions(userFromRepo.Id);
            claims.Add(new Claim(BeepClaimTypes.Permissions, defaultPermission.ToBits()));
            claims.Add(new Claim(BeepClaimTypes.PermissionsSerial, defaultPermission.Serial));
            claims.Add(new Claim(BeepClaimTypes.EnvironmentId, defaultPermission.Environment.Id.ToString()));

            _permissionsCache.AddEntriesForUser(defaultPermission.UserId, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds),
                await _authRepo.GetAllUserPermissions(userFromRepo.Id));

            return Ok(new
            {
                token = JwtHelper.CreateToken(claims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds)),
                mappedUser
            });
        }

        [HttpGet("UpdatePermissionClaims/{userId}")]
        public async Task<IActionResult> UpdatePermissionClaims(int userId, int environmentId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            Permission permissions = await _authRepo.GetUserPermissionForEnvironment(userId, environmentId);
            if (permissions == null) return Unauthorized();

            List<Claim> newClaims = User.Claims.Where(c => c.Type != BeepClaimTypes.Permissions &&
                                                           c.Type != BeepClaimTypes.PermissionsSerial &&
                                                           c.Type != BeepClaimTypes.EnvironmentId).ToList();
            newClaims.Add(new Claim(BeepClaimTypes.Permissions, permissions.ToBits()));
            newClaims.Add(new Claim(BeepClaimTypes.PermissionsSerial, permissions.Serial));
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

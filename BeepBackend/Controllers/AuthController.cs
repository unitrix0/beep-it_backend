using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IPermissionsCache _permissionsCache;
        private readonly string _tokenSecretKey;
        private readonly int _tokenLifeTimeSeconds;

        public AuthController(IAuthRepository authRepo, IUserRepository userRepo, IMapper mapper, IConfiguration config, IPermissionsCache permissionsCache)
        {
            _authRepo = authRepo;
            _userRepo = userRepo;
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
            var identityClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };
            IList<string> roles = await _authRepo.GetUserRoles(userFromRepo);
            identityClaims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var defaultPermission = await _authRepo.GetDefaultPermissions(userFromRepo.Id);
            List<Claim> permissionClaims = BuildPermissionClaims(defaultPermission);
            var settings = await GetSettings(userFromRepo.Id, user.Cameras);

            _permissionsCache.AddEntriesForUser(userFromRepo.Id,
                await _authRepo.GetAllUserPermissions(userFromRepo.Id));

            return Ok(new
            {
                identityToken = JwtHelper.CreateToken(identityClaims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds)),
                permissionsToken = JwtHelper.CreateToken(permissionClaims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds)),
                mappedUser,
                settings
            });
        }

        private async Task<SettingsDto> GetSettings(int userId, IEnumerable<CameraDto> userCameras)
        {
            string deviceId = await _userRepo.GetCamForUser(userId, userCameras.Select(uc => uc.DeviceId));

            return new SettingsDto()
            {
                CameraDeviceId = deviceId
            };
        }

        private List<Claim> BuildPermissionClaims(Permission permission)
        {
            var permissionClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, permission.UserId.ToString()),
                new Claim(BeepClaimTypes.Permissions, permission.ToBits()),
                new Claim(BeepClaimTypes.PermissionsSerial, permission.Serial),
                new Claim(BeepClaimTypes.EnvironmentId, permission.Environment.Id.ToString())
            };

            return permissionClaims;
        }

        [HttpGet("UpdatePermissionClaims/{userId}")]
        public async Task<IActionResult> UpdatePermissionClaims(int userId, int environmentId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            Permission permissions = await _authRepo.GetUserPermissionForEnvironment(userId, environmentId);
            if (permissions == null) return Unauthorized();

            var newClaims = BuildPermissionClaims(permissions);
            string newJwtToken = JwtHelper.CreateToken(newClaims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds));
            return new ObjectResult(new
            {
                permissionsToken = newJwtToken,
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

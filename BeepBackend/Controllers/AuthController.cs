using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Mailing;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utrix.WebLib;
using Utrix.WebLib.Helpers;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
        private readonly IBeepMailer _mailer;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly string _tokenSecretKey;
        private readonly int _tokenLifeTimeSeconds;

        public AuthController(IAuthRepository authRepo, IUserRepository userRepo, IMapper mapper, IConfiguration config,
            IPermissionsCache permissionsCache, IBeepMailer mailer, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _authRepo = authRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _permissionsCache = permissionsCache;
            _mailer = mailer;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenLifeTimeSeconds = Convert.ToInt32(config.GetSection("AppSettings:TokenLifeTime").Value);
            _tokenSecretKey = config.GetSection("AppSettings:Token").Value;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserForRegistrationDto newUser)
        {
            newUser.Username = newUser.Username.ToLower();

            var userToCreate = _mapper.Map<User>(newUser);
            IdentityResult createUsrResult = await _userManager.CreateAsync(userToCreate, newUser.Password);
            if (!createUsrResult.Succeeded) return BadRequest(createUsrResult.Errors);

            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(userToCreate, RoleNames.Member);
            if (!addRoleResult.Succeeded) BadRequest(addRoleResult.Errors);

            userToCreate = await _authRepo.CreateFirstEnvironment(userToCreate);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(userToCreate);
            await _mailer.SendConfirmationMail(userToCreate.Id, userToCreate.Email, token, false);

            var userToReturn = _mapper.Map<UserForEditDto>(userToCreate);
            Response.ExposeHeader("location");
            return CreatedAtRoute(nameof(UsersController.GetUser), new { controller = "Users", id = userToReturn.Id }, userToReturn);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserForLoginDto user)
        {
            User userFromRepo = await _userManager.FindByNameAsync(user.Username);
            if (userFromRepo == null) return Unauthorized();

            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(userFromRepo, user.Password, false);
            if (!signInResult.Succeeded) return Unauthorized(new { signInResult.IsLockedOut, signInResult.IsNotAllowed });

            var mappedUser = _mapper.Map<UserForTokenDto>(userFromRepo);
            var identityClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };
            IList<string> roles = await _userManager.GetRolesAsync(userFromRepo);
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

        [HttpPut("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string id, string token, string email, bool isChange)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new Exception("Failed to confirm the address");

            if (!isChange)
            {
                if (user.EmailConfirmed) return Ok();
                IdentityResult result = await _userManager.ConfirmEmailAsync(user, token.FromBase64());
                if (result.Succeeded) return Ok();
            }
            else
            {
                IdentityResult result = await _userManager.ChangeEmailAsync(user, email.FromBase64(), token.FromBase64());
                if (result.Succeeded) return Ok();
            }

            throw new Exception("Failed to confirm the address");
        }

        [HttpGet("ResendEmailConfirmation/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation(string username)
        {
            User user = await _userManager.FindByNameAsync(username);
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (User != null && !user.EmailConfirmed) await _mailer.SendConfirmationMail(user.Id, user.Email, token, false);

            return Ok();
        }

        [HttpGet("UpdatePermissionClaims/{userId}")]
        public async Task<IActionResult> UpdatePermissionClaims(int userId, int environmentId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            Permission permissions = await _authRepo.GetUserPermissionForEnvironment(userId, environmentId);
            if (permissions == null) return Unauthorized();

            var newClaims = BuildPermissionClaims(permissions);
            string newJwtToken = JwtHelper.CreateToken(newClaims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds));

            return Ok(new { permissionsToken = newJwtToken });
        }

        [HttpGet("UserExists/{userId}")]
        public async Task<IActionResult> UserExists(int userId, string username)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            User user = await _userManager.FindByNameAsync(username);
            return Ok(user != null);
        }



        private async Task<SettingsDto> GetSettings(int userId, IEnumerable<CameraDto> userCameras)
        {
            Camera cam = await _userRepo.GetCamForUser(userId, userCameras.Select(uc => uc.DeviceId));

            return new SettingsDto()
            {
                CameraDeviceId = cam?.DeviceId ?? "",
                CameraLabel = cam?.Label ?? ""
            };
        }

        private List<Claim> BuildPermissionClaims(Permission permission)
        {
            var permissionClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, permission.UserId.ToString()),
                new Claim(BeepClaimTypes.Permissions, permission.ToBits()),
                new Claim(BeepClaimTypes.PermissionsSerial, permission.Serial),
                new Claim(BeepClaimTypes.EnvironmentId, permission.EnvironmentId.ToString())
            };

            return permissionClaims;
        }
    }
}

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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Utrix.WebLib;
using Utrix.WebLib.Helpers;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
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
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly string _tokenSecretKey;
        private readonly IConfigurationSection _appSettings;
        private readonly int _tokenLifeTimeSeconds;

        public AuthController(IAuthRepository authRepo, IUserRepository userRepo, IMapper mapper, IConfiguration config,
            IPermissionsCache permissionsCache, IBeepMailer mailer, UserManager<User> userManager, SignInManager<User> signInManager,
            TokenValidationParameters tokenValidationParameters)
        {
            _authRepo = authRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _permissionsCache = permissionsCache;
            _mailer = mailer;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenValidationParameters = tokenValidationParameters;
            _appSettings = config.GetSection("AppSettings");
            _tokenSecretKey = config.GetSection("AppSettings:Token").Value;
            _tokenLifeTimeSeconds = Convert.ToInt32(_appSettings["TokenLifeTime"]);
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
            if (!addRoleResult.Succeeded) return BadRequest(addRoleResult.Errors);

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
            if (!signInResult.Succeeded) return Unauthorized(EvalLoginFailedReason(signInResult));

            var mappedUser = _mapper.Map<UserForTokenDto>(userFromRepo);

            var defaultPermission = await _authRepo.GetDefaultPermissions(userFromRepo.Id);
            List<Claim> permissionClaims = BuildPermissionClaims(defaultPermission);
            var settings = await GetSettings(userFromRepo.Id, user.Cameras);

            _permissionsCache.AddEntriesForUser(userFromRepo.Id,
                await _authRepo.GetAllUserPermissions(userFromRepo.Id));

            string refreshToken = await CreateRefreshToken(userFromRepo.Id);

            return Ok(new
            {
                identityToken = await CreateIdentityToken(userFromRepo),
                permissionsToken = JwtHelper.CreateToken(permissionClaims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(_tokenLifeTimeSeconds)),
                refreshToken,
                mappedUser,
                settings
            });
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(TokenRefreshDto refreshDto)
        {
            ClaimsPrincipal validToken = GetPrincipalFromToken(refreshDto.Token);

            if (validToken == null) return BadRequest("Token is not valid");

            var expiryDateUnix =
                long.Parse(validToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow) return BadRequest("This token hasn't expired yet");

            RefreshToken storedRefreshToken = await _authRepo.GetRefreshTokenForUser(refreshDto.RefreshToken);

            if (storedRefreshToken == null) return BadRequest("This refresh token does not exist");
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate) return BadRequest("This refresh token has expired");
            if (storedRefreshToken.Invalidated) return BadRequest("This refresh token has been invalidated");
            if (storedRefreshToken.Used) return BadRequest("This refresh token has been used");

            storedRefreshToken.Used = true;
            await _authRepo.SaveChangesAsync();

            string userId = validToken.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;
            User userFromRepo = await _userManager.FindByIdAsync(userId);
            string refreshToken = await CreateRefreshToken(userFromRepo.Id);

            return Ok(new
            {
                identityToken = await CreateIdentityToken(userFromRepo),
                refreshToken
            });
        }

        [HttpPost("DemoLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> DemoLogin()
        {
            int demoUserCount = await _authRepo.CountDemoUsers();
            demoUserCount++;

            var newUser = new User()
            {
                UserName = $"demo{demoUserCount}",
                DisplayName = "Demo Benutzer",
                Email = "demo@beep-it.ch",
                AccountExpireDate = DateTime.Now.AddSeconds(_tokenLifeTimeSeconds)
            };

            IdentityResult creationResult = await _userManager.CreateAsync(newUser, "P@ssw0rd");
            if (!creationResult.Succeeded) BadRequest(creationResult.Errors);

            IdentityResult addRoleResult = await _userManager.AddToRolesAsync(newUser, new[] { RoleNames.Member, RoleNames.Demo });
            if (!addRoleResult.Succeeded) return BadRequest(addRoleResult.Errors);

            newUser = await _authRepo.CreateDemoData(newUser);
            if (newUser == null) throw new Exception("Error creating environment/demo data");

            Permission defaultPermissions = await _authRepo.GetDefaultPermissions(newUser.Id);
            List<Claim> permissionClaims = BuildPermissionClaims(defaultPermissions);
            SettingsDto settings = await GetSettings(newUser.Id, new List<CameraDto>());

            _permissionsCache.AddEntriesForUser(newUser.Id, await _authRepo.GetAllUserPermissions(newUser.Id));
            var mappedUser = _mapper.Map<UserForTokenDto>(newUser);

            return Ok(new
            {
                identityToken = await CreateIdentityToken(newUser),
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
            if (user == null)
                throw new Exception("Failed to confirm the address");

            if (!isChange)
            {
                if (user.EmailConfirmed) return Ok();
                IdentityResult result = await _userManager.ConfirmEmailAsync(user, token.FromBase64());
                if (result.Succeeded) return Ok();
            } else
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
            var tokenLifeTimeSeconds = Convert.ToInt32(_appSettings["TokenLifeTime"]);
            if (!this.VerifyUser(userId)) return Unauthorized();

            Permission permissions = await _authRepo.GetUserPermissionForEnvironment(userId, environmentId);
            if (permissions == null) return Unauthorized();

            var newClaims = BuildPermissionClaims(permissions);
            string newJwtToken = JwtHelper.CreateToken(newClaims.ToArray(), _tokenSecretKey, DateTime.Now.AddSeconds(tokenLifeTimeSeconds));

            return Ok(new { permissionsToken = newJwtToken });
        }

        [HttpGet("UserExists/{userId}")]
        public async Task<IActionResult> UserExists(int userId, string username)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            User user = await _userManager.FindByNameAsync(username);
            return Ok(user != null);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string tokenString)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out var token);

                return !IsJwtWithValidSecurityAlgorithm(token) ? null : principal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken token)
        {
            return token is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<string> CreateIdentityToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_tokenSecretKey);
            var identityClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            IList<string> roles = await _userManager.GetRolesAsync(user);
            identityClaims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.Now.AddSeconds(_tokenLifeTimeSeconds),
                IssuedAt = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(identityClaims)
            };
            JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> CreateRefreshToken(int userId)
        {
            TimeSpan.TryParse(_appSettings["RefreshTokenLifetime"], out TimeSpan refreshTokenLifetime);
            var newToken = new RefreshToken()
            {
                CreationDate = DateTime.Now,
                ExpiryDate = DateTime.Now.Add(refreshTokenLifetime),
                UserId = userId,
                Token = Guid.NewGuid().ToString()
            };

            await _authRepo.AddRefreshToken(newToken);
            return newToken.Token;
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

        private string EvalLoginFailedReason(SignInResult signInResult)
        {
            if (signInResult.IsLockedOut)
                return "Login is locked";

            if (signInResult.IsNotAllowed)
                return "Login is not allowed";

            return "Bad username or password";
        }
    }
}

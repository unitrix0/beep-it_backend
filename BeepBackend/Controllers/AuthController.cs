using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BeepBackend.DTOs;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Utrix.WebLib.Authentication;

namespace BeepBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AuthControllerBase
    {
        private readonly IAuthRepository<User> _authRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository<User> authRepo, IMapper mapper, IConfiguration config)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserForRegistrationDto newUser)
        {
            newUser.Username = newUser.Username.ToLower();
            if (await _authRepo.UserExists(newUser.Username)) return BadRequest("Username already taken");

            var userToCreate = _mapper.Map<User>(newUser);
            User createdUser = await _authRepo.Register(userToCreate, newUser.Password);

            var userToReturn= _mapper.Map<UserForEditDto>(createdUser);
            return CreatedAtRoute(nameof(UsersController.GetUser), new {controller = "Users", id = createdUser.Id}, userToReturn);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserForLoginDto user)
        {
            var userFromRepo = await _authRepo.Login(user.Username.ToLower(), user.Password);
            if (userFromRepo == null) return Unauthorized();

            var mappedUser = _mapper.Map<UserForTokenDto>(userFromRepo);

            return Ok(new
            {
                token = CreateToken(userFromRepo.Id.ToString(), userFromRepo.UserName,
                    _config.GetSection("AppSettings:Token").Value),
                mappedUser
            });
        }
    }
}

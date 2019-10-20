using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Models;
using Utrix.WebLib.Authentication;

namespace BeepBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IBeepRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IBeepRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(int id)
        {
            if (!this.VerifyUser(id)) return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);
            if (userFromRepo == null) return Unauthorized();

            var userToReturn = _mapper.Map<UserForEditDto>(userFromRepo);
            return Ok(userToReturn);
        }


        [HttpPut("updatepermission/{userId}")]
        public async Task<IActionResult> UpdatePermission(int userId, [FromBody]PermissionsDto newPermission)
        {
            var environmentOwnerId = await _repo.GetEnvironmentOwnerId(newPermission.EnvironmentId);
            if (!this.VerifyUser(environmentOwnerId)) return Unauthorized();

            var currentPermission = await _repo.GetUserPermission(newPermission.EnvironmentId, newPermission.UserId);
            _mapper.Map(newPermission, currentPermission);
            currentPermission.Serial = AuthRepository.GeneratePermissionSerial();
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Error saving permissions user:{newPermission.UserId} env:{newPermission.EnvironmentId}");
        }

        [HttpPost("addenvironment/{userId}")]
        public async Task<IActionResult> AddEnvironment(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            var newEnv = await _repo.AddEnvironment(userId);
            if (newEnv == null) return BadRequest("Error creating environment");

            var newEnvDto = _mapper.Map<EnvironmentDto>(newEnv);
            return Ok(new { newEnvDto });
        }

        [HttpGet("GetEnvironments/{userId}")]
        public async Task<IActionResult> GetEnvironments(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            var envs = await _repo.GetEnvironments(userId);
            var envsDto = _mapper.Map<IEnumerable<BeepEnvironment>,IEnumerable<EnvironmentDto>>(envs);

            return Ok(envsDto);
        }
    }
}

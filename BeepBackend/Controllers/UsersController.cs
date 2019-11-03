using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly IPermissionsCache _permissionsCache;

        public UsersController(IBeepRepository repo, IMapper mapper, IPermissionsCache permissionsCache)
        {
            _repo = repo;
            _mapper = mapper;
            _permissionsCache = permissionsCache;
        }

        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(int id)
        {
            if (!this.VerifyUser(id)) return Unauthorized();

            User userFromRepo = await _repo.GetUser(id);
            if (userFromRepo == null) return Unauthorized();

            var userToReturn = _mapper.Map<UserForEditDto>(userFromRepo);
            return Ok(userToReturn);
        }


        [HttpPut("updatepermission")]
        public async Task<IActionResult> UpdatePermission([FromBody]PermissionsDto newPermission)
        {
            int environmentOwnerId = await _repo.GetEnvironmentOwnerId(newPermission.EnvironmentId);
            if (!this.VerifyUser(environmentOwnerId)) return Unauthorized();

            Permission currentPermission = await _repo.GetUserPermission(newPermission.EnvironmentId, newPermission.UserId);
            _mapper.Map(newPermission, currentPermission);
            currentPermission.Serial = SerialGenerator.Generate();
            _permissionsCache.Update(currentPermission.User.UserName, currentPermission.Environment.Id,
                currentPermission.Serial);
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Error saving permissions user:{newPermission.UserId} env:{newPermission.EnvironmentId}");
        }

        [HttpPost("addenvironment/{userId}")]
        public async Task<IActionResult> AddEnvironment(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            BeepEnvironment newEnv = await _repo.AddEnvironment(userId);
            if (newEnv == null) return BadRequest("Error creating environment");

            var newEnvDto = _mapper.Map<EnvironmentDto>(newEnv);
            return Ok(newEnvDto);
        }

        [HttpPost("DeleteEnvironment/{userId}/{envId}")]
        public async Task<IActionResult> DeleteEnvironment(int userId, int envId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            if (await _repo.DeleteEnvironment(envId)) return NoContent();

            throw new Exception("Failed deleting the Environment");
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteMember(InvitationDto invitation)
        {
            int environmentOwnerId = await _repo.GetEnvironmentOwnerId(invitation.EnvironmentId);
            if (!this.VerifyUser(environmentOwnerId)) return Unauthorized();

            if (invitation.SendMail)
            {
                Invitation createdInvitation = await _repo.InviteMemberByMail(invitation.InviteeName, invitation.EnvironmentId);
                if (createdInvitation != null)
                {
                    // Wenn keine Serial generiert wurde, ist der Benutzer bereits vorhanden
                    if (createdInvitation.Serial == string.Empty) return NoContent();
                    //TODO Send Mail
                    return NoContent();
                }
            }
            else
            {
                if (await _repo.InviteMember(invitation.InviteeName, invitation.EnvironmentId))
                    return NoContent();
            }

            throw new Exception("Failed to create invitation");
        }

        [HttpGet("InvitationsCount/{userId}")]
        public async Task<IActionResult> InvitationsCount(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            int count = await _repo.CountInvitations(userId);
            return Ok(count);
        }

        [HttpPost("AnswerInvitation/{userId}")]
        public async Task<IActionResult> AnswerInvitation(int userId, int environmentId, int answer)
        {
            int inviteeId = await _repo.GetInviteeId(userId, environmentId);
            if (!this.VerifyUser(inviteeId)) return Unauthorized();

            if (answer == 0)
            {
                if (await _repo.DeleteInvitation(userId, environmentId)) return NoContent();
            }
            else if (answer == 1)
            {
                if (await _repo.JoinEnvironment(userId, environmentId)) return NoContent();
            }


            throw new Exception("Failed to answer invitation");
        }

        [HttpDelete("DeleteInvitation/{userId}")]
        public async Task<IActionResult> DeleteInvitation(int userId, int environmentId, int inviteeId)
        {
            int inviterId = await _repo.GetInviterId(environmentId, inviteeId);
            if (!this.VerifyUser(userId) || !this.VerifyUser(inviterId)) return Unauthorized();

            if (await _repo.DeleteInvitation(inviteeId, environmentId)) return Ok();

            throw new Exception("Error deleting invitation");
        }

        [HttpDelete("DeleteAnsweredInvitations/{userId}")]
        public async Task<IActionResult> DeleteAnsweredInvitations(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            if (await _repo.DeleteAnsweredInvitations(userId)) return NoContent();

            throw new Exception("Error deleting the invitations");
        }

        [HttpGet("Invitations/{userId}")]
        public async Task<IActionResult> GetInvitationsForUser(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            var invitations = new InvitationsDto();

            List<Invitation> invitationsReceived = await _repo.GetReceivedInvitationsForUser(userId);
            invitations.ReceivedInvitations = _mapper.Map<IEnumerable<InvitationListItemDto>>(invitationsReceived);

            List<Invitation> invitationsSent = await _repo.GetSentInvitationsForUserAsync(userId);
            invitations.SentInvitations = _mapper.Map<IEnumerable<InvitationListItemDto>>(invitationsSent);

            return Ok(invitations);
        }

    }
}

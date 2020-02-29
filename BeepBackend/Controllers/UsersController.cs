using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BeepBackend.Helpers;
using BeepBackend.Mailing;
using Utrix.WebLib.Helpers;

namespace BeepBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPermissionsCache _permissionsCache;
        private readonly IAuthorizationService _authService;
        private readonly UserManager<User> _userManager;
        private readonly IBeepMailer _mailer;

        public UsersController(IUserRepository repo, IMapper mapper, IPermissionsCache permissionsCache,
            IAuthorizationService authService, UserManager<User> userManager, IBeepMailer mailer)
        {
            _repo = repo;
            _mapper = mapper;
            _permissionsCache = permissionsCache;
            _authService = authService;
            _userManager = userManager;
            _mailer = mailer;
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


        [HttpPut("SetPermission")]
        public async Task<IActionResult> SetPermission([FromBody]PermissionsDto newPermission)
        {
            int environmentOwnerId = await _repo.GetEnvironmentOwnerId(newPermission.EnvironmentId);
            //TODO Umbauen?
            AuthorizationResult authorization = await _authService.AuthorizeAsync(User, null,
                new[]
                {
                    new ChangePermissionRequirement(newPermission.EnvironmentId, environmentOwnerId,newPermission.UserId,
                        PermissionFlags.IsOwner | PermissionFlags.ManageUsers)
                });

            if (!authorization.Succeeded) return Unauthorized();

            Permission currentPermission = await _repo.GetUserPermission(newPermission.EnvironmentId, newPermission.UserId);
            _mapper.Map(newPermission, currentPermission);
            currentPermission.Serial = SerialGenerator.Generate();
            _permissionsCache.Update(currentPermission.UserId, currentPermission.Environment.Id, currentPermission);

            if (await _repo.SaveAll()) return NoContent();

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
            if (!this.VerifyUser(userId) ||
                !await _authService.IsPermitted(User, envId, PermissionFlags.IsOwner)) return Unauthorized();

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
            //TODO Prüfen ob Einladung überhaupt vorhanden
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

            List<Invitation> invitationsSent = await _repo.GetSentInvitationsForUser(userId);
            invitations.SentInvitations = _mapper.Map<IEnumerable<InvitationListItemDto>>(invitationsSent);

            return Ok(invitations);
        }

        [HttpDelete("RemoveUser/{userId}")]
        public async Task<IActionResult> RemoveUser(int userId, int environmentId, int removeUserId)
        {
            if (!this.VerifyUser(await _repo.GetEnvironmentOwnerId(environmentId))) return Unauthorized();

            if (await _repo.RemoveUserFromEnvironmentAsync(environmentId, removeUserId)) return Ok();

            throw new Exception("Error removing user");
        }

        [HttpGet("GetEnvironments/{userId}")]
        public async Task<IActionResult> GetEnvironments(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            IEnumerable<BeepEnvironment> environments = await _repo.GetEnvironments(userId);
            var result = _mapper.Map<IEnumerable<EnvironmentDto>>(environments);

            return Ok(result);
        }

        [HttpGet("GetEnvironmentPermissions/{userId}")]
        public async Task<IActionResult> GetEnvironmentPermissions(int userId, int environmentId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            IEnumerable<Permission> permissions = await _repo.GetEnvironmentPermissions(environmentId, userId);
            var result = _mapper.Map<IEnumerable<PermissionsDto>>(permissions);

            return Ok(result);
        }

        [HttpPut("UpdateEnvironmentName/{envId}")]
        public async Task<IActionResult> UpdateEnvironmentName(int envId, string newName)
        {
            if (!await _authService.IsPermitted(User, envId, PermissionFlags.IsOwner)) return Unauthorized();

            BeepEnvironment environment = await _repo.GetEnvironment(envId);
            environment.Name = newName;

            if (await _repo.SaveAll()) return Ok();

            throw new Exception("Error updating Environment name");
        }

        [HttpPost("AddCamForUser/{userId}")]
        public async Task<IActionResult> AddCamForUser(int userId, CameraDto camDto)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            var cam = _mapper.Map<Camera>(camDto);
            if (!await _repo.AddCamForUser(userId, cam)) throw new Exception("Error adding cam for user");

            var camCreated = _mapper.Map<CameraDto>(cam);
            return StatusCode((int)HttpStatusCode.Created, camCreated);
        }

        [HttpGet("GetSettings/{userId}")]
        public async Task<IActionResult> GetSettings(int userId)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            List<Camera> cams = await _repo.GetCamsForUser(userId);
            var result = new SettingsDto()
            {
                //Cameras = _mapper.Map<IEnumerable<CameraDto>>(cams)
            };

            return Ok(result);
        }

        [HttpPut("SetDisplayName/{userId}")]
        public async Task<IActionResult> SetDisplayName(int userId, [FromBody] ProfileChangeDto changesDto)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            User user = await _repo.GetUser(userId);
            user.DisplayName = changesDto.DisplayName;

            if (await _repo.SaveAll()) return NoContent();

            throw new Exception("Error updating display name");
        }

        [HttpPut("SetEmailAddress/{userId}")]
        public async Task<IActionResult> SetEmailAddress(int userId, [FromBody] ProfileChangeDto changesDto)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            User user = await _repo.GetUser(userId);
            string token = await _userManager.GenerateChangeEmailTokenAsync(user, changesDto.EMail);
            await _mailer.SendConfirmationMail(user.Id, changesDto.EMail, token, true);

            return Ok();
        }

        [HttpPut("ChangePassword/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] ProfileChangeDto changesDto)
        {
            if (!this.VerifyUser(userId)) return Unauthorized();

            bool changed = await _repo.ChangePassword(userId, changesDto.CurrentPassword, changesDto.Password);

            if (changed) return NoContent();
            throw new Exception("Error updating E-Mail address");
        }
    }
}

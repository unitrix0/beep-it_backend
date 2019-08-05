using System.Threading.Tasks;
using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utrix.WebLib.Pagination;

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
    }
}

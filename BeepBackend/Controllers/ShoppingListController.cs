using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace BeepBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListController : ControllerBase
    {
        private readonly IShoppingListRepo _repo;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authService;

        public ShoppingListController(IShoppingListRepo repo, IMapper mapper, IAuthorizationService authService)
        {
            _repo = repo;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet("GetShoppingList/{environmentId}")]
        public async Task<IActionResult> GetShoppingList(int environmentId)
        {
            if (!await _authService.IsPermitted(User, environmentId)) return Unauthorized();

            IEnumerable<ShoppingListEntry> articles = await _repo.GetShoppingListAsync(environmentId);

            IEnumerable<ShoppingListEntryDto> list = _mapper.Map<IEnumerable<ShoppingListEntryDto>>(articles);
            return Ok(list);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeepBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleRepository _repo;
        private readonly IMapper _mapper;

        public ArticlesController(IArticleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{environmentId}")]
        public async Task<IActionResult> GetArticles(int environmentId, [FromQuery]ArticleFilter filter)
        {
            List<Article> articles = await _repo.GetArticles(environmentId, filter);

            var articlesDto = _mapper.Map<IEnumerable<ArticleDto>>(articles);

            return Ok(articlesDto);
        }
    }
}

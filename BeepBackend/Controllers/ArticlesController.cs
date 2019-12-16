using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utrix.WebLib.Pagination;

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
            PagedList<Article> articles = await _repo.GetArticles(environmentId, filter);

            IEnumerable<ArticleDto> articlesDto = _mapper.Map<IEnumerable<ArticleDto>>(articles);
            Response.AddPagination(articles.CurrentPage, articles.PageSize, articles.TotalCount, articles.TotalPages);

            return Ok(articlesDto);
        }

        [HttpGet("GetBaseData")]
        public async Task<IActionResult> GetBaseData()
        {
            IEnumerable<ArticleUnit> units = await _repo.GetUnits();
            IEnumerable<ArticleGroup> articleGroups = await _repo.GetArticleGroups();

            return Ok(new { units, articleGroups });
        }

        [HttpGet("LookupArticle")]
        public async Task<IActionResult> LookupArticle(string barcode, int environmentId)
        {
            var entry = await _repo.GetStockEntryForArticle(barcode, environmentId);
            if (entry == null) return Ok(new {});

            return Ok(entry);
        }
    }
}

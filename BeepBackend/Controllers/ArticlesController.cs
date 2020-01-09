using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utrix.WebLib.Pagination;

namespace BeepBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //TODO Check Permissions
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

            IEnumerable<EditArticleDto> articlesDto = _mapper.Map<IEnumerable<EditArticleDto>>(articles);
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

        [HttpGet("LookupArticle/{barcode}/{environmentId}", Name = nameof(LookupArticle))]
        public async Task<IActionResult> LookupArticle(string barcode, int environmentId)
        {
            Article article = await _repo.LookupArticle(barcode);
            if (article == null) return Ok(new EditArticleDto());

            ArticleUserSetting userSetting = await _repo.LookupArticleUserSettings(article.Id, environmentId);
            var articleDto = _mapper.Map<EditArticleDto>(article);
            articleDto.ArticleUserSettings = _mapper.Map<ArticleUserSettingDto>(userSetting);

            return Ok(articleDto);
        }

        [HttpPost("SaveArticle")]
        public async Task<IActionResult> SaveArticle(EditArticleDto newArticleDto)
        {
            var article = _mapper.Map<Article>(newArticleDto);
            var userSettings = _mapper.Map<ArticleUserSetting>(newArticleDto.ArticleUserSettings);

            Article createdArticle = await _repo.SaveArticle(article, userSettings);
            var createdArticleDto = _mapper.Map<EditArticleDto>(createdArticle);

            return CreatedAtRoute(nameof(LookupArticle),
                new
                {
                    controller = "Articles",
                    barcode = createdArticle.Barcode,
                    environmentId = newArticleDto.ArticleUserSettings.EnvironmentId
                }, createdArticleDto);
        }

        [HttpPost("AddStockEntry")]
        public async Task<IActionResult> AddStockEntry(CheckInDto checkInDto)
        {
            var entryValues = _mapper.Map<StockEntryValue>(checkInDto);
            entryValues.ExpireDate = entryValues.ExpireDate.AddMinutes(checkInDto.ClientTimezoneOffset);

            StockEntryValue newEntry = await _repo.AddStockEntry(entryValues, checkInDto.UsualLifetime);
            var ret = _mapper.Map<EditArticleDto>(newEntry.Article);

            return CreatedAtRoute(nameof(LookupArticle),
                new { controller = "Articles", barcode = checkInDto.Barcode, environmentId = checkInDto.EnvironmentId }, ret);
        }

        [HttpGet("GetArticleDateSuggestions/{barcode}/{environmentId}")]
        public async Task<IActionResult> GetArticleDateSuggestions(string barcode, int environmentId)
        {
            long usualLifetime = await _repo.GetArticleLifetime(barcode, environmentId);
            DateTime lastExpireDate = await _repo.GetLastExpireDate(barcode, environmentId);
            return Ok(new { usualLifetime, lastExpireDate });
        }

        [HttpGet("GetArticleStock")]
        public async Task<IActionResult> GetArticleStock(int articleId, int environmentId, int pageNumber, int itemsPerPage)
        {
            PagedList<StockEntryValue> stockEntries =
                await _repo.GetStockEntries(articleId, environmentId, pageNumber, itemsPerPage);

            IEnumerable<StockEntryValueDto>
                stockEntriesDto = _mapper.Map<IEnumerable<StockEntryValueDto>>(stockEntries);
            Response.AddPagination(stockEntries.CurrentPage, stockEntries.PageSize, stockEntries.TotalCount, stockEntries.TotalPages);

            return Ok(stockEntriesDto);
        }

        [HttpGet("GetCheckOutArticle")]
        public async Task<IActionResult> GetCheckOutArticle(int environmentId, string barcode)
        {
            StockEntryValue entry = await _repo.GetOldestStockEntryValue(barcode, environmentId);
            var entryDto = _mapper.Map<StockEntryValueDto>(entry);

            return Ok(entryDto);
        }

        [HttpDelete("CheckOut")]
        public async Task<IActionResult> CheckOut(int environmentId, string barcode)
        {
            StockEntryValue entry = await _repo.GetOldestStockEntryValue(barcode, environmentId);
            if (entry.AmountOnStock == 1)
            {
                _repo.Delete(entry);
                return await _repo.SaveAll() ? NoContent(): throw new Exception("Failed to delete the stock entry");
            }

            entry.AmountOnStock--;
            return await _repo.SaveAll() ? NoContent() : throw new Exception("Failed to update stock entry");

        }

        [HttpDelete("CheckOutById")]
        public async Task<IActionResult> CheckOutById(int entryId, int amount)
        {
            StockEntryValue entry = await _repo.GetStockEntryValue(entryId);
            if (amount == 0 || entry.AmountOnStock == 1 || entry.AmountOnStock == amount)
            {
                _repo.Delete(entry);
                return await _repo.SaveAll() ? NoContent() : throw new Exception("Failed to delete the stock entry");
            }

            entry.AmountOnStock -= amount;
            return await _repo.SaveAll() ? NoContent() : throw new Exception("Failed to update stock entry");
        }
    }
}

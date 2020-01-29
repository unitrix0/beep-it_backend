using AutoMapper;
using BeepBackend.Data;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using BeepBackend.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
        private readonly IAuthorizationService _authService;

        public ArticlesController(IArticleRepository repo, IMapper mapper, IAuthorizationService authService)
        {
            _repo = repo;
            _mapper = mapper;
            _authService = authService;
        }

        [HttpGet("{environmentId}")]
        public async Task<IActionResult> GetArticles(int environmentId, [FromQuery]ArticleFilter filter)
        {
            if (!await _authService
                .IsPermitted(User, environmentId,
                    PermissionFlags.IsOwner | PermissionFlags.EditArticleSettings))
                return Unauthorized();

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
            if (!await _authService.IsPermitted(User, environmentId, PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            Article article = await _repo.LookupArticle(barcode);
            if (article == null) return Ok(new EditArticleDto());

            ArticleUserSetting userSetting = await _repo.LookupArticleUserSettings(article.Id, environmentId);
            var articleDto = _mapper.Map<EditArticleDto>(article);
            articleDto.ArticleUserSettings = _mapper.Map<ArticleUserSettingDto>(userSetting);

            return Ok(articleDto);
        }

        [HttpPost("CreateArticle")]
        public async Task<IActionResult> CreateArticle(EditArticleDto newArticleDto)
        {
            if (!await _authService.IsPermitted(User, 0,
                PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            var article = _mapper.Map<Article>(newArticleDto);
            var userSettings = _mapper.Map<ArticleUserSetting>(newArticleDto.ArticleUserSettings);

            Article createdArticle = await _repo.CreateArticle(article, userSettings);
            var createdArticleDto = _mapper.Map<EditArticleDto>(createdArticle);

            return CreatedAtRoute(nameof(LookupArticle),
                new
                {
                    controller = "Articles",
                    barcode = createdArticle.Barcode,
                    environmentId = newArticleDto.ArticleUserSettings.EnvironmentId
                }, createdArticleDto);
        }

        [HttpPatch("UpdateArticle")]
        public async Task<IActionResult> UpdateArticle(EditArticleDto articleDto)
        {
            if (!await _authService.IsPermitted(User, 0,
                PermissionFlags.IsOwner | PermissionFlags.EditArticleSettings)) return Unauthorized();

            Article article = await _repo.GetArticle(articleDto.Id);
            ArticleUserSetting articleSettings = await _repo.GetArticleUserSettings(articleDto.ArticleUserSettings.Id);
            _mapper.Map(articleDto, article);
            _mapper.Map(articleDto.ArticleUserSettings, articleSettings);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception("Failed to update article");
        }

        [HttpPost("AddStockEntry")]
        public async Task<IActionResult> AddStockEntry(CheckInDto checkInDto)
        {
            if (!await _authService.IsPermitted(User, 0,
                PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            checkInDto.ExpireDate = checkInDto.ExpireDate.AddMinutes(checkInDto.ClientTimezoneOffset);
            var entryValues = _mapper.Map<StockEntryValue>(checkInDto);
            entryValues.AmountRemaining = 1;

            StockEntryValue newEntry = await _repo.AddStockEntry(entryValues, checkInDto.UsualLifetime);
            var ret = _mapper.Map<EditArticleDto>(newEntry.Article);

            return CreatedAtRoute(nameof(LookupArticle),
                new { controller = "Articles", barcode = checkInDto.Barcode, environmentId = checkInDto.EnvironmentId }, ret);
        }

        [HttpGet("GetArticleDateSuggestions/{barcode}/{environmentId}")]
        public async Task<IActionResult> GetArticleDateSuggestions(string barcode, int environmentId)
        {
            if (!await _authService.IsPermitted(User, environmentId,
               PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            long usualLifetime = await _repo.GetArticleLifetime(barcode, environmentId);
            DateTime lastExpireDate = await _repo.GetLastExpireDate(barcode, environmentId);
            return Ok(new { usualLifetime, lastExpireDate });
        }

        [HttpGet("GetArticleStock")]
        public async Task<IActionResult> GetArticleStock(int articleId, int environmentId, int pageNumber, int itemsPerPage)
        {
            if (!await _authService.IsPermitted(User, environmentId,
                PermissionFlags.IsOwner | PermissionFlags.CanScan | PermissionFlags.EditArticleSettings))
                return Unauthorized();

            PagedList<StockEntryValue> stockEntries =
                await _repo.GetStockEntries(articleId, environmentId, pageNumber, itemsPerPage);

            IEnumerable<StockEntryValueDto>
                stockEntriesDto = _mapper.Map<IEnumerable<StockEntryValueDto>>(stockEntries);
            Response.AddPagination(stockEntries.CurrentPage, stockEntries.PageSize, stockEntries.TotalCount, stockEntries.TotalPages);

            return Ok(stockEntriesDto);
        }

        [HttpDelete("CheckOutById")]
        public async Task<IActionResult> CheckOutById(int entryId, int amount)
        {
            StockEntryValue entry = await _repo.GetStockEntryValue(entryId);
            if (entry == null) return NotFound();

            if (!await _authService.IsPermitted(User, entry.EnvironmentId,
               PermissionFlags.IsOwner | PermissionFlags.EditArticleSettings)) return Unauthorized();

            if (amount == 0 || entry.AmountOnStock == 1 || entry.AmountOnStock == amount)
            {
                _repo.Delete(entry);
                return await _repo.SaveAll() ? NoContent() : throw new Exception("Failed to delete the stock entry");
            }

            entry.AmountOnStock -= amount;
            return await _repo.SaveAll() ? NoContent() : throw new Exception("Failed to update stock entry");
        }

        [HttpPut("OpenArticle")]
        public async Task<IActionResult> OpenArticle(StockEntryValueDto stockEntryDto)
        {
            StockEntryValue existingEntry = await _repo.GetStockEntryValue(stockEntryDto.Id);

            if (!await _authService.IsPermitted(User, existingEntry.EnvironmentId,
               PermissionFlags.IsOwner | PermissionFlags.CanScan | PermissionFlags.EditArticleSettings))
                return Unauthorized();

            stockEntryDto.OpenedOn = stockEntryDto.OpenedOn.AddMinutes(stockEntryDto.ClientTimezoneOffset);
            if (existingEntry.AmountOnStock == 1)
            {
                _mapper.Map(stockEntryDto, existingEntry);
                if (await _repo.SaveAll()) return NoContent();

                throw new Exception("Error saving the Data");
            }

            var newEntry = _mapper.Map<StockEntryValue>(stockEntryDto);
            newEntry.Id = 0;
            newEntry.AmountOnStock = 1;

            existingEntry.AmountOnStock--;

            if (await _repo.CreateStockEntryValue(newEntry)) return NoContent();

            throw new Exception("Error saving the Data");
        }
    }
}

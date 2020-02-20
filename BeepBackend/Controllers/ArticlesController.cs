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
using System.Globalization;
using System.Threading.Tasks;
using Utrix.WebLib;
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

        [HttpGet("GetArticles")]
        public async Task<IActionResult> GetArticles([FromQuery]ArticleFilter filter)
        {
            if (!await _authService
                .IsPermitted(User, filter.EnvironmentId))
                return Unauthorized();

            PagedList<Article> articles = await _repo.GetArticles(filter);

            IEnumerable<ArticleDto> articlesDto = _mapper.Map<IEnumerable<ArticleDto>>(articles);
            Response.AddPagination(articles.CurrentPage, articles.PageSize, articles.TotalCount, articles.TotalPages);

            return Ok(articlesDto);
        }

        [HttpGet("LookupArticle/{barcode}", Name = nameof(LookupArticle))]
        public async Task<IActionResult> LookupArticle(string barcode)
        {
            Article article = await _repo.LookupArticle(barcode);
            if (article == null) return Ok(new ArticleDto());

            var articleDto = _mapper.Map<ArticleDto>(article);
            return Ok(articleDto);
        }

        [HttpGet("GetArticleUserSettings", Name = nameof(GetArticleUserSettings))]
        public async Task<IActionResult> GetArticleUserSettings(int articleId, int environmentId)
        {
            if (!await _authService.IsPermitted(User, environmentId)) return Unauthorized();

            ArticleUserSetting aus = await _repo.GetArticleUserSettings(articleId, environmentId);
            return aus == null ? Ok(new ArticleUserSettingDto()) : Ok(aus);
        }

        [HttpGet("GetBaseData")]
        public async Task<IActionResult> GetBaseData()
        {
            IEnumerable<ArticleUnit> units = await _repo.GetUnits();
            IEnumerable<ArticleGroup> articleGroups = await _repo.GetArticleGroups();
            IEnumerable<Store> stores = await _repo.GetStores();

            return Ok(new { units, articleGroups, stores });
        }

        [HttpPost("CreateArticle")]
        public async Task<IActionResult> CreateArticle(ArticleDto newArticleDto)
        {
            var article = _mapper.Map<Article>(newArticleDto);

            Article createdArticle = await _repo.CreateArticle(article);
            var createdArticleDto = _mapper.Map<ArticleDto>(createdArticle);

            return CreatedAtRoute(nameof(LookupArticle),
                new
                {
                    controller = "Articles",
                    barcode = createdArticle.Barcode,
                }, createdArticleDto);
        }

        [HttpPost("CreateArticleUserSettings")]
        public async Task<IActionResult> CreateArticleUserSettings(ArticleUserSettingDto articleUserSettingDto)
        {
            if (!await _authService.IsPermitted(User, articleUserSettingDto.EnvironmentId,
                PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            var articleUserSetting = _mapper.Map<ArticleUserSetting>(articleUserSettingDto);
            ArticleUserSetting articleUserSettingCreated = await _repo.CreateArticleUserSetting(articleUserSetting);

            var dto = _mapper.Map<ArticleUserSettingDto>(articleUserSettingCreated);
            return CreatedAtRoute(nameof(GetArticleUserSettings), new
            {
                controller = "Articles",
                articleId = articleUserSettingCreated.ArticleId,
                environmentId = articleUserSettingCreated.EnvironmentId
            }, dto);
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

        [HttpPatch("UpdateArticle")]
        public async Task<IActionResult> UpdateArticle(ArticleDto articleDto)
        {
            Article article = await _repo.GetArticle(articleDto.Id);
            _mapper.Map(articleDto, article);

            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception("Failed to update article");
        }

        [HttpPost("AddStockEntry")]
        public async Task<IActionResult> AddStockEntry(CheckInDto checkInDto)
        {
            if (!await _authService.IsPermitted(User, checkInDto.EnvironmentId,
                PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            checkInDto.ExpireDate = checkInDto.ExpireDate.AddMinutes(checkInDto.ClientTimezoneOffset);
            var entryValues = _mapper.Map<StockEntryValue>(checkInDto);
            entryValues.AmountRemaining = 1;

            StockEntryValue newEntry = await _repo.AddStockEntry(entryValues, checkInDto.UsualLifetime);
            var ret = _mapper.Map<ArticleDto>(newEntry.Article);

            await _repo.WriteActivityLog(ActivityAction.CheckIn, User, checkInDto.EnvironmentId, checkInDto.ArticleId, checkInDto.AmountOnStock.ToString());

            return CreatedAtRoute(nameof(LookupArticle),
                new { controller = "Articles", barcode = checkInDto.Barcode, environmentId = checkInDto.EnvironmentId }, ret);
        }

        [HttpGet("GetArticleStock")]
        public async Task<IActionResult> GetArticleStock(int articleId, int environmentId, int pageNumber, int itemsPerPage)
        {
            if (!await _authService.IsPermitted(User, environmentId,
                PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            PagedStockList stockEntries =
                await _repo.GetStockEntries(articleId, environmentId, pageNumber, itemsPerPage);

            IEnumerable<StockEntryValueDto>
                stockEntriesDto = _mapper.Map<IEnumerable<StockEntryValueDto>>(stockEntries);
            Response.AddPagination(stockEntries.CurrentPage, stockEntries.PageSize, stockEntries.TotalCount, stockEntries.TotalPages);
            Response.AddCustomHeader("TotalStockAmount", stockEntries.TotalStockAmount.ToString());

            return Ok(stockEntriesDto);
        }

        [HttpDelete("CheckOutById")]
        public async Task<IActionResult> CheckOutById(int entryId, int amount)
        {
            StockEntryValue entry = await _repo.GetStockEntryValue(entryId);
            if (entry == null) return NotFound();

            if (!await _authService.IsPermitted(User, entry.EnvironmentId,
               PermissionFlags.IsOwner | PermissionFlags.CanScan)) return Unauthorized();

            if (amount == 0 || entry.AmountOnStock == 1 || entry.AmountOnStock == amount)
            {
                _repo.Delete(entry);
                if (!await _repo.SaveAll()) throw new Exception("Failed to delete the stock entry");

                await _repo.WriteActivityLog(ActivityAction.CheckOut, User, entry.EnvironmentId, entry.ArticleId,
                    entry.AmountRemaining.ToString(CultureInfo.CurrentCulture));
                return NoContent();
            }

            await _repo.WriteActivityLog(ActivityAction.CheckOut, User, entry.EnvironmentId, entry.ArticleId,
                amount.ToString());

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

            await _repo.WriteActivityLog(ActivityAction.Open, User, existingEntry.EnvironmentId, existingEntry.ArticleId,
                newEntry.AmountRemaining.ToString(CultureInfo.CurrentCulture));

            if (await _repo.CreateStockEntryValue(newEntry)) return NoContent();

            throw new Exception("Error saving the Data");
        }

        [HttpGet("GetActivityLog/{environmentId}")]
        public async Task<IActionResult> GetActivityLog(int environmentId)
        {
            if (!await _authService.IsPermitted(User, environmentId)) return Unauthorized();

            IEnumerable<ActivityLogEntry> entries = await _repo.GetActivityLog(environmentId);

            var entriesDto = _mapper.Map<IEnumerable<ActivityLogEntryDto>>(entries);

            return Ok(entriesDto);
        }
    }
}

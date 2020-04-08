using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BeepBackend.DTOs;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Utrix.WebLib.Pagination;

namespace BeepBackend.Data
{
    public interface IArticleRepository
    {
        Task<PagedList<Article>> GetArticles(ArticleFilter filter);
        Task<IEnumerable<ArticleUnit>> GetUnits();
        Task<IEnumerable<ArticleGroup>> GetArticleGroups(int userId);
        Task<Article> LookupArticle(string barcode);
        Task<long> GetArticleLifetime(string barcode, int environmentId);
        Task<Article> CreateArticle(Article article);
        Task<StockEntryValue> AddStockEntry(StockEntryValue entryValues, long usualLifetime);
        Task<PagedStockList> GetStockEntries(int articleId, int environmentId, int page, int itemsPerPage);
        Task<DateTime> GetLastExpireDate(string barcode, int environmentId);
        Task<StockEntryValue> GetOldestStockEntryValue(string barcode, int environmentId);
        void Delete<T>(T entry) where T : class;
        Task<bool> SaveAll();
        Task<StockEntryValue> GetStockEntryValue(int entryId);
        Task<Article> GetArticle(int articleId);
        Task<ArticleUserSetting> GetArticleUserSettings(int articleId, int environmentId);
        Task<bool> CreateStockEntryValue(StockEntryValue newEntry);
        Task<IEnumerable<Store>> GetStores();
        Task<ArticleUserSetting> CreateArticleUserSetting(ArticleUserSetting articleUserSetting);
        Task WriteActivityLog(ActivityLogAction logAction, ClaimsPrincipal user, int environmentId, int articleId, string amount);
        Task<IEnumerable<ActivityLogEntry>> GetActivityLog(int environmentId);
        Task<ArticleGroup> CreateArticleGroup(ArticleGroup newGroup);
        /// <summary>
        /// Ruft ide <see cref="ArticleGroup"/> mit der angegebenen Id ab.
        /// Falls gewünscht werden die verknüpften <see cref="ArticleUserSetting"/>
        /// auch abgefragt.
        /// </summary>
        /// <param name="articleGroupId"></param>
        /// <param name="expanded"><c>true</c> wenn die <see cref="ArticleUserSetting"/> auch abgefragt werden sollen</param>
        /// <returns></returns>
        Task<ArticleGroup> GetArticleGroup(int articleGroupId, bool expanded);
        Task<int> GetArticleGroupMemberCount(int groupId);
        Task<int> GetEnvironmentOwnerId(int environmentId);
    }
}
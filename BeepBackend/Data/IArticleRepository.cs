using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Utrix.WebLib.Pagination;

namespace BeepBackend.Data
{
    public interface IArticleRepository
    {
        Task<PagedList<Article>> GetArticles(int environmentId, ArticleFilter filter);
        Task<IEnumerable<ArticleUnit>> GetUnits();
        Task<IEnumerable<ArticleGroup>> GetArticleGroups();
        Task<Article> LookupArticle(string barcode);
        Task<ArticleUserSetting> LookupArticleUserSettings(int articleId, int environmentId);
        Task<long> GetArticleLifetime(string barcode, int environmentId);
        Task<Article> CreateArticle(Article article, ArticleUserSetting userSettings);
        Task<StockEntryValue> AddStockEntry(StockEntryValue entryValues, long usualLifetime);
        Task<PagedList<StockEntryValue>> GetStockEntries(int articleId, int environmentId, int page, int itemsPerPage);
        Task<DateTime> GetLastExpireDate(string barcode, int environmentId);
        Task<StockEntryValue> GetOldestStockEntryValue(string barcode, int environmentId);
        void Delete<T>(T entry) where T : class;
        Task<bool> SaveAll();
        Task<StockEntryValue> GetStockEntryValue(int entryId);
        Task<Article> GetArticle(int articleId);
        Task<ArticleUserSetting> GetArticleUserSettings(int id);
    }
}
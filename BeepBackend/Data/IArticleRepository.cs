﻿using System.Collections.Generic;
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
        Task<ArticleUserSetting> LookupArticleUserSettings(string barcode, int environmentId);
        Task<Article> SaveArticle(Article article, ArticleUserSetting userSettings);
        Task<StockEntryValue> AddStockEntry(StockEntryValue entryValues, int usualLifetime);
    }
}
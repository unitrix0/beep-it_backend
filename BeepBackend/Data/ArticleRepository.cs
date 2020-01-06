using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utrix.WebLib.Pagination;
using StockEntryValue = BeepBackend.Models.StockEntryValue;

namespace BeepBackend.Data
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly BeepDbContext _context;

        public ArticleRepository(BeepDbContext context)
        {
            _context = context;
        }

        public void Delete<T>(T entry) where T : class
        {
            _context.Remove(entry);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<Article>> GetArticles(int environmentId, ArticleFilter filter)
        {
            IQueryable<Article> articles = filter.StoreId > 0
                ? _context.ArticleStores
                    .Where(ast => ast.StoreId == filter.StoreId)
                    .Select(ast => ast.Article)
                : _context.Articles.AsQueryable();

            articles = articles
                .Include(a => a.ArticleUserSettings)
                .Include(a => a.StockEntryValues)
                .Where(a => a.ArticleUserSettings.Any(aus => aus.EnvironmentId == environmentId));

            if (filter.IsOnStock)
            {
                List<int> articlesOnStock = await _context.StockEntries
                    .Where(se => se.EnvironmentId == environmentId)
                    .Select(se => se.ArticleId).ToListAsync();

                articles = articles.Where(a => articlesOnStock.Contains(a.Id));
            }

            if (filter.IsOpened)
            {
                var articlesOpened = await _context.StockEntryValues
                    .Where(sev => sev.IsOpened && sev.EnvironmentId == environmentId)
                    .Select(sev => sev.ArticleId)
                    .ToListAsync();

                articles = articles.Where(a => articlesOpened.Contains(a.Id));
            }

            if (filter.KeepOnStock)
            {
                var keepStockArticles = await _context.ArticleUserSettings
                    .Where(aus => aus.KeepStockAmount > 0 && aus.EnvironmentId == environmentId)
                    .Select(aus => aus.ArticleFk).ToListAsync();

                articles = articles.Where(a => keepStockArticles.Contains(a.Id));
            }

            if (!string.IsNullOrEmpty(filter.NameOrEan))
            {
                var envArticles = await _context.ArticleUserSettings
                    .Where(aus => aus.EnvironmentId == environmentId)
                    .Select(aus => aus.ArticleFk).ToListAsync();

                articles = articles
                    .Where(a => a.Barcode.Contains(filter.NameOrEan) || a.Name.Contains(filter.NameOrEan) &&
                                envArticles.Contains(a.Id));
            }

            //TODO noch benötigt?
            //if (filter.StoreId > 0)
            //{
            //    var stores = await _context.ArticleStores
            //        .Where(ast => ast.StoreId == filter.StoreId)
            //        .Select(ast => ast.ArticleId).ToListAsync();

            //    articles = articles.Where(a => stores.Contains(a.Id));
            //}


            return await PagedList<Article>.CreateAsync(articles, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<ArticleUnit>> GetUnits()
        {
            return await _context.ArticleUnits.ToListAsync();
        }

        public async Task<IEnumerable<ArticleGroup>> GetArticleGroups()
        {
            return await _context.ArticleGroups.ToListAsync();
        }

        public async Task<Article> LookupArticle(string barcode)
        {
            Article article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Barcode == barcode);

            return article;
        }

        public async Task<ArticleUserSetting> LookupArticleUserSettings(int articleId, int environmentId)
        {
            ArticleUserSetting userSettings = await _context.ArticleUserSettings
                .FirstOrDefaultAsync(aus => aus.ArticleFk == articleId &&
                                            aus.EnvironmentId == environmentId);

            return userSettings;
        }

        public async Task<int> GetArticleLifetime(string barcode, int environmentId)
        {
            ArticleUserSetting userSettings = await _context.ArticleUserSettings
                .Include(aus => aus.Article)
                .FirstOrDefaultAsync(aus => aus.Article.Barcode == barcode &&
                                            aus.EnvironmentId == environmentId);

            return userSettings.UsualLifetime;
        }

        public async Task<DateTime> GetLastExpireDate(string barcode, int environmentId)
        {
            StockEntryValue lastExpireDate = await _context.StockEntryValues
                .Include(sev => sev.Article)
                .Where(sev => sev.Article.Barcode == barcode &&
                              sev.EnvironmentId == environmentId)
                .OrderBy(sev => sev.ExpireDate).FirstOrDefaultAsync();

            return lastExpireDate?.ExpireDate ?? DateTime.Now;
        }

        public async Task<Article> SaveArticle(Article article, ArticleUserSetting userSettings)
        {
            userSettings.Article = article;
            await _context.ArticleUserSettings.AddAsync(userSettings);
            await _context.Articles.AddAsync(article);

            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<StockEntryValue> AddStockEntry(StockEntryValue entryValues, int usualLifetime)
        {
            StockEntry stockEntry = await _context.StockEntries
                .Include(se => se.Article).ThenInclude(a => a.ArticleUserSettings)
                .FirstOrDefaultAsync(se => se.EnvironmentId == entryValues.EnvironmentId &&
                                           se.ArticleId == entryValues.ArticleId);

            if (stockEntry == null)
            {
                stockEntry = new StockEntry()
                {
                    EnvironmentId = entryValues.EnvironmentId,
                    ArticleId = entryValues.ArticleId
                };
                await _context.StockEntries.AddAsync(stockEntry);
                await _context.StockEntryValues.AddAsync(entryValues);
                entryValues.StockEntry = stockEntry;
            }
            else
            {
                StockEntryValue existingEntryValues = await _context.StockEntryValues
                    .FirstOrDefaultAsync(sev => sev.ArticleId == entryValues.ArticleId &&
                                                sev.ExpireDate == entryValues.ExpireDate);

                if (existingEntryValues == null)
                {
                    await _context.StockEntryValues.AddAsync(entryValues);
                    entryValues.StockEntry = stockEntry;
                }
                else
                {
                    existingEntryValues.AmountOnStock += entryValues.AmountOnStock;
                }
            }

            ArticleUserSetting articleUserSetting = await _context.ArticleUserSettings
                .Include(aus => aus.Article)
                .FirstOrDefaultAsync(aus => aus.Article.Id == entryValues.ArticleId);

            articleUserSetting.UsualLifetime = usualLifetime;

            await _context.SaveChangesAsync();
            return entryValues;
        }

        public async Task<PagedList<StockEntryValue>> GetStockEntries(int articleId, int environmentId, int page, int itemsPerPage)
        {
            IQueryable<StockEntryValue> qry = _context.StockEntryValues
                .Where(se => se.EnvironmentId == environmentId && se.ArticleId == articleId)
                .OrderBy(se => se.ExpireDate).ThenByDescending(se => se.IsOpened);

            return await PagedList<StockEntryValue>.CreateAsync(qry, page, itemsPerPage);
        }

        public async Task<StockEntryValue> GetOldestStockEntryValue(string barcode, int environmentId)
        {
            StockEntryValue entry = await _context.StockEntryValues
                .Include(sev => sev.Article)
                .OrderBy(sev => sev.ExpireDate)
                .FirstOrDefaultAsync(sev => sev.EnvironmentId == environmentId &&
                                            sev.Article.Barcode == barcode);

            return entry;
        }
        
        public async Task<StockEntryValue> GetStockEntryValue(int entryId)
        {
            StockEntryValue entry = await _context.StockEntryValues
                .FirstOrDefaultAsync(sev => sev.Id == entryId);

            return entry;
        }
    }
}

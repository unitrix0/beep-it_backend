using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utrix.WebLib.Pagination;

namespace BeepBackend.Data
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly BeepDbContext _context;

        public ArticleRepository(BeepDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Article>> GetArticles(int environmentId, ArticleFilter filter)
        {
            IQueryable<Article> articles = filter.StoreId > 0
                ? _context.ArticleStores
                    .Where(ast => ast.StoreId == filter.StoreId)
                    .Select(ast => ast.Article)
                : _context.Articles.AsQueryable();

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

            //if (filter.StoreId > 0)
            //{
            //    var stores = await _context.ArticleStores
            //        .Where(ast => ast.StoreId == filter.StoreId)
            //        .Select(ast => ast.ArticleId).ToListAsync();

            //    articles = articles.Where(a => stores.Contains(a.Id));
            //}


            return await PagedList<Article>.CreateAsync(articles.Include(a => a.ArticleUserSettings), filter.PageNumber, filter.PageSize);
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

        public async Task<ArticleUserSetting> LookupArticleUserSettings(string barcode, int environmentId)
        {
            ArticleUserSetting userSettings = await _context.ArticleUserSettings
                .Include(aus => aus.Article)
                .FirstOrDefaultAsync(aus => aus.Article.Barcode == barcode);

            return userSettings;
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
                ArticleUserSetting articleUserSetting = await _context.ArticleUserSettings
                    .Include(aus => aus.Article)
                    .FirstOrDefaultAsync(aus => aus.Article.Id == entryValues.ArticleId);

                articleUserSetting.UsualLifetime = usualLifetime;
                stockEntry = new StockEntry()
                {
                    EnvironmentId = entryValues.EnvironmentId,
                    ArticleId = entryValues.ArticleId
                };
                await _context.StockEntries.AddAsync(stockEntry);
            }

            entryValues.StockEntry = stockEntry;
            await _context.StockEntryValues.AddAsync(entryValues);
            await _context.SaveChangesAsync();

            return entryValues;
        }
    }
}

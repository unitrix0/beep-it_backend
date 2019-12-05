﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var articlesOpened = await _context.StockEntries
                    .Where(se => se.IsOpened && se.EnvironmentId == environmentId)
                    .Select(se => se.ArticleId)
                    .ToListAsync();

                articles = articles.Where(a => articlesOpened.Contains(a.Id));
            }

            if (filter.KeepStock)
            {
                var keepStockArticles = await _context.ArticleUserSettings
                    .Where(aus => aus.KeepStockAmount > 0 && aus.EnvironmentId == environmentId)
                    .Select(aus => aus.ArticleFk).ToListAsync();

                articles = articles.Where(a => keepStockArticles.Contains(a.Id));
            }

            if (!string.IsNullOrEmpty(filter.NameEan))
            {
                var envArticles = await _context.ArticleUserSettings
                    .Where(aus => aus.EnvironmentId == environmentId)
                    .Select(aus => aus.ArticleFk).ToListAsync();

                articles = articles
                    .Where(a => a.Barcode.Contains(filter.NameEan) || a.Name.Contains(filter.NameEan) &&
                                envArticles.Contains(a.Id));
            }

            //if (filter.StoreId > 0)
            //{
            //    var stores = await _context.ArticleStores
            //        .Where(ast => ast.StoreId == filter.StoreId)
            //        .Select(ast => ast.ArticleId).ToListAsync();

            //    articles = articles.Where(a => stores.Contains(a.Id));
            //}
            

            return await PagedList<Article>.CreateAsync(articles, filter.PageNumber, filter.PageSize);
        }
    }
}

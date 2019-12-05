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
    }
}
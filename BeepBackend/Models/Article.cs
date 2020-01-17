using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string Barcode { get; set; }
        public bool HasLifetime { get; set; }
        public string ImageUrl { get; set; }
        public int ContentAmount { get; set; }

        public int ArticleGroupFk { get; set; }
        public ArticleGroup ArticleGroup { get; set; }

        public int UnitId { get; set; }
        public ArticleUnit Unit { get; set; }

        public ICollection<ArticleUserSetting> ArticleUserSettings { get; set; }
        public ICollection<ArticleStore> Stores { get; set; }
        public ICollection<UserArticle> UserArticles { get; set; }
        public ICollection<StockEntry> StockEntries { get; set; }
        public ICollection<StockEntryValue> StockEntryValues { get; set; }
    }
}

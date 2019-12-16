using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utrix.WebLib.Pagination;

namespace BeepBackend.Helpers
{
    public class ArticleFilter : PaginationParams
    {
        public int StoreId { get; set; }
        public bool IsOpened { get; set; }
        public bool KeepOnStock { get; set; }
        public bool IsOnStock { get; set; }
        /// <summary>
        /// Name oder EAN Nummer
        /// </summary>
        public string NameOrEan { get; set; }

        public ArticleFilter() : base(12, 12)
        {
            NameOrEan = "";
        }
    }
}

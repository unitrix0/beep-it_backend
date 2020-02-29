﻿using Utrix.WebLib.Pagination;

namespace BeepBackend.DTOs
{
    public class ArticleFilter : PaginationParams
    {
        public int EnvironmentId { get; set; }
        public int StoreId { get; set; }
        public bool IsOpened { get; set; }
        public bool KeepOnStock { get; set; }
        public bool IsOnStock { get; set; }
        /// <summary>
        /// Name oder EAN Nummer eines gesuchten Artikels
        /// </summary>
        public string NameOrEan { get; set; }

        public ArticleFilter() : base(12, 12)
        {
            NameOrEan = "";
        }
    }
}

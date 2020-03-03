using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.DTOs
{
    public class ShoppingListArticleEntryDto
    {
        public string Barcode { get; set; }
        public string ArticleName { get; set; }
        public string ImageUrl { get; set; }
        public string UnitAbbreviation { get; set; }
        public int KeepStockAmount { get; set; }
        public int OnStock { get; set; }
        public int Opened { get; set; }
        public int Needed { get; set; }
        public float AmountRemaining { get; set; }
    }
}

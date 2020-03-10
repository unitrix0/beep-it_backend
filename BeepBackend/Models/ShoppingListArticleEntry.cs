using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeepBackend.Models
{
    public class ShoppingListArticleEntry
    {
        public string Barcode { get; set; }
        public string StoreName { get; set; }
        public string ArticleName { get; set; }
        public string ImageUrl { get; set; }
        public string UnitAbbreviation { get; set; }
        public int EnvironmentId { get; set; }
        public int KeepStockAmount { get; set; }
        public int OnStock { get; set; }
        public int Opened { get; set; }
        public int Needed { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal AmountRemaining { get; set; }
    }
}
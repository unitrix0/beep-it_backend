using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeepBackend.Models
{
    public class StockEntryValue
    {
        public int Id { get; set; }

        public bool IsOpened { get; set; }
        public int AmountOnStock { get; set; }
        public DateTime OpenedOn { get; set; }
        public DateTime ExpireDate { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public float AmountRemaining { get; set; }

        public int EnvironmentId { get; set; }
        public int ArticleId { get; set; }
        public Article Article { get; set; }
        public StockEntry StockEntry { get; set; }
    }
}
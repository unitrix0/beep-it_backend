using System;

namespace BeepBackend.DTOs
{
    public class StockEntryValueDto
    {
        public bool IsOpened { get; set; }
        public int AmountOnStock { get; set; }
        public DateTime OpenedOn { get; set; }
        public DateTime ExpireDate { get; set; }
        public float AmountRemaining { get; set; }
        public int ArticleId { get; set; }
    }
}
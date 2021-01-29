﻿using System;

namespace BeepBackend.Articles.DTOs
{
    public class StockEntryValueDto
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public bool IsOpened { get; set; }
        public int AmountOnStock { get; set; }
        public DateTime OpenedOn { get; set; }
        public DateTime ExpireDate { get; set; }
        public float AmountRemaining { get; set; }
        public int ArticleId { get; set; }
        public int ClientTimezoneOffset { get; set; }
    }
}
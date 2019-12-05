﻿using System;

namespace BeepBackend.Models
{
    public class StockEntry
    {
        public bool IsOpened { get; set; }
        public int AmountOnStock { get; set; }
        public DateTime OpenedOn { get; set; }
        public DateTime ExpireDate { get; set; }
        public float AmountRemaining { get; set; }

        public Article Article { get; set; }
        public BeepEnvironment Environment { get; set; }
        public int EnvironmentId { get; set; }
        public int ArticleId { get; set; }
    }
}
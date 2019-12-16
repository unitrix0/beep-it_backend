using System;
using System.Collections.Generic;

namespace BeepBackend.Models
{
    public class StockEntry
    {
        public Article Article { get; set; }
        public int ArticleId { get; set; }
        public BeepEnvironment Environment { get; set; }
        public int EnvironmentId { get; set; }

        public ICollection<StockEntryValue> StockEntryValues { get; set; }
    }
}
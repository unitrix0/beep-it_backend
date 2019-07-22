using System;

namespace BeepBackend.Models
{
    public class ArticleUserSetting
    {
        public int Id { get; set; }
        public int StockAmount { get; set; }
        public int KeppStockMode { get; set; }
        public bool IsOpened { get; set; }
        public DateTime OpenedOn { get; set; }
        public int AmountOnStock { get; set; }

        public int ArticleFk { get; set; }
        public Article Article { get; set; }
        public int EnvironmentFk { get; set; }
        public Environment Environment { get; set; }
    }
}

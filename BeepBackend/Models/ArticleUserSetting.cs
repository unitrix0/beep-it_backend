namespace BeepBackend.Models
{
    public class ArticleUserSetting
    {
        public int Id { get; set; }
        public int KeepStockMode { get; set; }
        public int KeepStockAmount { get; set; }
        public long UsualLifetime { get; set; }

        public int ArticleFk { get; set; }
        public Article Article { get; set; }

        public int UnitId { get; set; }
        public ArticleUnit Unit { get; set; }
        public int EnvironmentId { get; set; }
        public BeepEnvironment Environment { get; set; }
    }
}

namespace BeepBackend.Models
{
    public class ArticleUserSetting
    {
        public int Id { get; set; }
        public int KeepStockMode { get; set; }
        public int KeepStockAmount { get; set; }
        public long UsualLifetime { get; set; }

        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public int EnvironmentId { get; set; }
        public BeepEnvironment Environment { get; set; }

        public int ArticleGroupId { get; set; }
        public ArticleGroup ArticleGroup { get; set; }
    }
}

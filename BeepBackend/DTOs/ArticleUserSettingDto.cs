namespace BeepBackend.DTOs
{
    public class ArticleUserSettingDto
    {
        public int Id { get; set; }
        public int EnvironmentId { get; set; }
        public int KeepStockMode { get; set; }
        public int KeepStockAmount { get; set; }
    }
}
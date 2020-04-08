namespace BeepBackend.DTOs
{
    public class ArticleUserSettingDto
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public int EnvironmentId { get; set; }
        public int KeepStockMode { get; set; } = 1;
        public int KeepStockAmount { get; set; } = 0;

        public int ArticleGroupId { get; set; }
        public ArticleGroupDto ArticleGroup { get; set; } = new ArticleGroupDto();
    }
}
namespace BeepBackend.DTOs
{
    public class UpdateArticleDto
    {
        public ArticleDto Article { get; set; }
        public ArticleUserSettingDto ArticleUserSettings { get; set; }
    }
}
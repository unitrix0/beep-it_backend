using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.DTOs
{
    public class EditArticleDto
    {
        public int Id { get; set; }
        public string Barcode { set; get; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public int UnitId { get; set; }
        public int ContentAmount { get; set; }
        public bool HasLifetime { get; set; }
        public DateTime NextExpireDate { get; set; }
        public string ImageUrl { get; set; }
        public ArticleUserSettingDto ArticleUserSettings { get; set; }
        public int TotalStockAmount { get; set; }

        public EditArticleDto()
        {
            ArticleUserSettings = new ArticleUserSettingDto();
        }
    }
}

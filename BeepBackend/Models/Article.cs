using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Name{ get; set; }
        public string Barcode { get; set; }
        public int TypicalLifetime { get; set; }
        public bool HasLifetime { get; set; }

        public int ArticleGroupFk { get; set; }
        public ArticleGroup ArticleGroup { get; set; }
        public ICollection<ArticleUserSetting> ArticleUserSettings { get; set; }
    }
}

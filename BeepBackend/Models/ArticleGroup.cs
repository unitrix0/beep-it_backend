using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class ArticleGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int KeepStockAmount { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
        
        public ICollection<ArticleUserSetting> ArticleUserSettings { get; set; }
    }
}

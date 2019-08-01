using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class ArticleStore
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }

        public int ArticleId { get; set; }
        public Article Article { get; set; }
    }
}

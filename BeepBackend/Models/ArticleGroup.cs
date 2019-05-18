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

        public ICollection<Article> Articles { get; set; }
    }
}

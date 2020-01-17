using System.Collections.Generic;

namespace BeepBackend.Models
{
    public class ArticleUnit
    {
        public int Id { get; set; }
        public string Abbreviation { get; set; }
        public string Name { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
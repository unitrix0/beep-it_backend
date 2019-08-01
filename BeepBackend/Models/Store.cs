using System.Collections.Generic;

namespace BeepBackend.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ArticleStore> Articles { get; set; }
    }
}
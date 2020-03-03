using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class ShoppingListEntry
    {
        public string StoreName { get; set; }
        public IEnumerable<ShoppingListArticleEntry> Articles { get; set; }
    }
}

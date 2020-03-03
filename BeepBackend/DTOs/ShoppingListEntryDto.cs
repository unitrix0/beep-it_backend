using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.DTOs
{
    public class ShoppingListEntryDto
    {
        public string StoreName { get; set; }
        public IEnumerable<ShoppingListArticleEntryDto> Articles { get; set; }
    }
}

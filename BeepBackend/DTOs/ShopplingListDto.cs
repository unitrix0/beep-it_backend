using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.DTOs
{
    public class ShopplingListDto
    {
        public IEnumerable<ShoppingListEntryDto> ArticleEntries { get; set; }
        public IEnumerable<ShoppingListGroupEntryDto> GroupEntries { get; set; }
    }
}

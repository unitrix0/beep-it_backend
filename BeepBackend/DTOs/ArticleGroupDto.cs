using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.DTOs
{
    public class ArticleGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int KeepStockAmount { get; set; }
    }
}

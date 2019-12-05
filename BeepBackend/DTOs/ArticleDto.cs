using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.DTOs
{
    public class ArticleDto
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public int TypicalLifetime { get; set; }
        public bool HasLifetime { get; set; }
        public int GroupId { get; set; }
        public string ImageUrl { get; set; }
    }
}

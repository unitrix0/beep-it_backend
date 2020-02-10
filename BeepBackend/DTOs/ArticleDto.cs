using System;
using System.Collections.Generic;

namespace BeepBackend.DTOs
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Barcode { set; get; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public int UnitId { get; set; }
        public int ContentAmount { get; set; }
        public bool HasLifetime { get; set; }
        public DateTime NextExpireDate { get; set; }
        public string ImageUrl { get; set; }

        public IEnumerable<ArticleStoreDto> Stores { get; set; }
    }
}

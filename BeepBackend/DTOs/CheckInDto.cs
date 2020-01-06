using System;

namespace BeepBackend.DTOs
{
    public class CheckInDto
    {
        public int AmountOnStock { get; set; }
        public DateTime ExpireDate { get; set; }
        public int UsualLifetime { get; set; }
        public int EnvironmentId { get; set; }
        public string Barcode { get; set; }
        public int ArticleId { get; set; }
    }
}
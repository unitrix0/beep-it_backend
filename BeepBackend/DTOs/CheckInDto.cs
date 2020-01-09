using System;

namespace BeepBackend.DTOs
{
    public class CheckInDto
    {
        public int AmountOnStock { get; set; }
        public DateTime ExpireDate { get; set; }
        public long UsualLifetime { get; set; }
        public int EnvironmentId { get; set; }
        public string Barcode { get; set; }
        public int ArticleId { get; set; }
        public double ClientTimezoneOffset { get; set; }
    }
}
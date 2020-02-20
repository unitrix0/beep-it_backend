using System;

namespace BeepBackend.DTOs
{
    public class ActivityLogEntryDto
    {
        public int Action { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string Amount { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
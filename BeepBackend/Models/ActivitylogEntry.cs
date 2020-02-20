using System;

namespace BeepBackend.Models
{
    public class ActivityLogEntry
    {
        public int Id { get; set; }
        public int Action { get; set; }
        public string ImgUrl { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public string Amount { get; set; }
        public DateTime ActionDate { get; set; }

        public int EnvironmentId { get; set; }
        public BeepEnvironment Environment { get; set; }
    }
}

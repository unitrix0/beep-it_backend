﻿namespace BeepBackend.DTOs
{
    public class UserForTokenDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class UserCamera
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int CameraId { get; set; }
        public Camera Camera { get; set; }
    }
}

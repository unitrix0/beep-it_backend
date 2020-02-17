using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace BeepBackend.Models
{
    public class Camera
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public string Label { get; set; }
        public ICollection<UserCamera> Users { get; set; }
    }
}

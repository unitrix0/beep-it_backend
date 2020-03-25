using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeepBackend.DTOs
{
    public class AddCameraDto
    {
        public CameraDto Camera { get; set; }
        public string OldCamDeviceId { get; set; }
    }
}

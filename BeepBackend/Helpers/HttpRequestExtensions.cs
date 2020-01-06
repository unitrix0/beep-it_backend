using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BeepBackend.Helpers
{
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Ermittelt den Wert des "ClientTimezoneOffset" wertes im Request Header. Gibt bei Fehler 0 zurück.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Wert von "ClientTimezoneOffset" oder 0 bei Exception</returns>
        public static int GetClientTimzoneOffset(this HttpRequest request)
        {
            try
            {
                return int.Parse(request.Headers["ClientTimezoneOffset"]);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}

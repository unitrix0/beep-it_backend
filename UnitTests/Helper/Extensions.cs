using BeepBackend.DTOs;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using BeepBackend.Helpers;
using UnitTests.DTOs;
using Utrix.WebLib.Helpers;

namespace UnitTests.Helper
{
    internal static class Extensions
    {
        /// <summary>
        /// Meldet sich mit dem angegebenen Benutzer an. Gibt null zurück falls login fehlschlägt
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="subUrl"></param>
        /// <returns></returns>
        public static LoginResponseObject Login(this HttpClient client, string username, string password, string subUrl = "auth/login")
        {
            HttpResponseMessage response = client.PostAsJsonAsync(subUrl, new UserForLoginDto
            {
                Username = username,
                Password = password,
                Cameras = new List<CameraDto>()
            }).Result;

            if (!response.IsSuccessStatusCode) return null;

            var content = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            JwtSecurityToken permissionsToken = JwtHelper.DecodeToken(content.SelectToken("permissionsToken").Value<string>());
            Claim serialClaim = permissionsToken.Claims.FirstOrDefault(c => c.Type == BeepClaimTypes.PermissionsSerial);
            Claim envIdClaim = permissionsToken.Claims.FirstOrDefault(c => c.Type == BeepClaimTypes.EnvironmentId);
            client.DefaultRequestHeaders.Remove("PermissionsSerial");
            client.DefaultRequestHeaders.Remove("EnvironmentId");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", content.SelectToken("identityToken").Value<string>());
            client.DefaultRequestHeaders.Add("PermissionsSerial",serialClaim?.Value);
            client.DefaultRequestHeaders.Add("EnvironmentId",envIdClaim?.Value);


            return content.ToObject<LoginResponseObject>();
        }


        /// <summary>
        /// Setzt den verwendeten Bearer Token sowie PermissionsSerial und EnvironmentId Header
        /// im <see cref="client"/>.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="login">Login Objekt mit den Token Informationen</param>
        /// <returns></returns>
        public static HttpClient UseLogin(this HttpClient client, LoginResponseObject login)
        {
            Claim serialClaim = JwtHelper.DecodeToken(login.PermissionsToken).Claims
                .FirstOrDefault(c => c.Type == BeepClaimTypes.PermissionsSerial);
            Claim envId = JwtHelper.DecodeToken(login.PermissionsToken).Claims
                .FirstOrDefault(c => c.Type == BeepClaimTypes.EnvironmentId);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.IdentityToken);
            client.DefaultRequestHeaders.Remove("PermissionsSerial");
            client.DefaultRequestHeaders.Remove("EnvironmentId");
            client.DefaultRequestHeaders.Add("PermissionsSerial", serialClaim?.Value);
            client.DefaultRequestHeaders.Add("EnvironmentId", envId?.Value);
            return client;
        }

        public static async Task<string> GetStringByQueryAsync(this HttpClient client, string requestUri, object valueObj)
        {
            var qry = new QueryBuilder(valueObj.ToKeyValuePairs());

            return await client.GetStringAsync(requestUri + qry);
        }

        public static async Task<HttpResponseMessage> GetAsyncQuery(this HttpClient client, string requestUri,
            object valueObj)
        {
            var qry = new QueryBuilder(valueObj.ToKeyValuePairs());

            return await client.GetAsync(requestUri + qry);
        }

        public static async Task<HttpResponseMessage> DeleteAsyncQuery(this HttpClient client, string requestUri,
            object valueObj)
        {
            var qry = new QueryBuilder(valueObj.ToKeyValuePairs());

            return await client.DeleteAsync(requestUri + qry);
        }

        public static string ToBits(this PermissionsDto permission)
        {
            var values = new List<short>()
            {
                Convert.ToInt16(permission.ManageUsers),
                Convert.ToInt16(permission.EditArticleSettings),
                Convert.ToInt16(permission.CanScan),
                Convert.ToInt16(permission.IsOwner)
            };

            return string.Join("", values);
        }

        private static Dictionary<string, string> ToKeyValuePairs(this object source)
        {
            Dictionary<string, string> valuePairs = source.GetType().GetProperties()
                .Select(p => new { p.Name, value = p.GetValue(source).ToString() })
                .ToDictionary(x => x.Name, x => x.value);

            return valuePairs;
        }
    }
}

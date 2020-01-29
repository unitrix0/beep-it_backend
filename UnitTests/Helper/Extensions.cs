using BeepBackend.DTOs;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnitTests.DTOs;

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
                Password = password
            }).Result;

            if (!response.IsSuccessStatusCode) return null;

            var content = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", content.SelectToken("token").Value<string>());

            return content.ToObject<LoginResponseObject>();
        }


        /// <summary>
        /// Setzt den verwendeten Bearer Token im <see cref="client"/>.
        /// Hiermit kann der verwendete Login definiert werden.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static HttpClient UseToken(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

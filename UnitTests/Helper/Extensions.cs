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

        public static HttpClient UseToken(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public static async Task<string> GetByQuery(this HttpClient client, string requestUri, object valueObj)
        {
            Dictionary<string, string> valuePairs = valueObj.GetType().GetProperties()
                .Select(p => new { p.Name, value = p.GetValue(valueObj).ToString() })
                .ToDictionary(x => x.Name, x => x.value);

            var qry = new QueryBuilder(valuePairs);

            return await client.GetStringAsync(requestUri + qry);
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
    }
}

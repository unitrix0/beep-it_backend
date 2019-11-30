using System;
using System.Collections.Generic;
using BeepBackend.DTOs;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using BeepBackend.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTests
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

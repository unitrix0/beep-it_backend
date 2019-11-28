using BeepBackend.DTOs;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace UnitTests
{
    internal static class Extensions
    {
        public static UserForTokenDto Login(this HttpClient client, string username, string password, string subUrl = "auth/login")
        {
            HttpResponseMessage response = client.PostAsJsonAsync(subUrl, new UserForLoginDto
            {
                Username = username,
                Password = password
            }).Result;

            if (!response.IsSuccessStatusCode) return null;

            var content = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", content.SelectToken("token").Value<string>());

            return content.SelectToken("mappedUser").ToObject<UserForTokenDto>();
        }
    }
}

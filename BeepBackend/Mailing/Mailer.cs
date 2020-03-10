using BeepBackend.Helpers;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BeepBackend.Mailing
{
    public class Mailer : IBeepMailer
    {
        private readonly IMailerClient _client;
        private readonly IConfiguration _config;

        public Mailer(IMailerClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public Task SendConfirmationMail(int userId, string email, string token, bool isChange)
        {
            var message = new StringBuilder();
            message.AppendLine("Please click the following link to confirm your email:")
                .AppendLine()
                .Append(_config.GetSection("AppSettings:FrontendBaseUrl").Value)
                .Append($"/AccountActivation/?email={WebUtility.UrlEncode(email.ToBase64())}&")
                .Append($"id={userId}&")
                .Append($"isChange={isChange}&")
                .Append($"token={WebUtility.UrlEncode(token.ToBase64())}");

            var subject = isChange ? "Beep-it change email" : "Beep-it email confirmation link";

            return _client.Send(email, subject, message.ToString(), message.ToString());
        }

    }
}


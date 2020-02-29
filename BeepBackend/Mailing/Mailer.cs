using System.Net;
using System.Text;
using System.Threading.Tasks;
using BeepBackend.Helpers;
using BeepBackend.Models;
using Microsoft.Extensions.Configuration;

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

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return _client.Send(email, subject, htmlMessage, htmlMessage);
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

            return SendEmailAsync(email, subject, message.ToString());
        }
    }
}


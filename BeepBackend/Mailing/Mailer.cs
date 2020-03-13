using BeepBackend.Data;
using BeepBackend.Helpers;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace BeepBackend.Mailing
{
    public class Mailer : IBeepMailer
    {
        private readonly IMailerClient _client;
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;

        public Mailer(IMailerClient client, IConfiguration config, IUserRepository userRepo)
        {
            _client = client;
            _config = config;
            _userRepo = userRepo;
        }

        public Task SendConfirmationMail(int userId, string email, string token, bool isChange)
        {
            var user = _userRepo.GetUser(userId).Result;

            var confirmationLink = new StringBuilder();
            confirmationLink.AppendLine(_config.GetSection("AppSettings:FrontendBaseUrl").Value)
                .Append($"/AccountActivation/?email={WebUtility.UrlEncode(email.ToBase64())}&")
                .Append($"id={userId}&")
                .Append($"isChange={isChange}&")
                .Append($"token={WebUtility.UrlEncode(token.ToBase64())}");

            var subject = isChange ? "Beep-it E-Mail Adresse ändern" : "Beep-it E-Mail Adresse bestätigen";

            return _client.Send(email, subject, GetConfirmationMessage(confirmationLink.ToString(), user.DisplayName, user.Email));
        }

        private string GetConfirmationMessage(string confirmationLink, string userDisplayName, string emailAddress)
        {
            string mail = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Mail Templates",
                "email_confirmation.html"));

            mail = mail.Replace("{{Name}}", userDisplayName);
            mail = mail.Replace("{{Link}}", confirmationLink);
            mail = mail.Replace("{{email}}", emailAddress);

            return mail;
        }
    }
}


using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BeepBackend.Mailing
{
    public class SendGridMailerClient : IMailerClient
    {
        private readonly IConfiguration _config;

        public SendGridMailerClient(IConfiguration config)
        {
            _config = config.GetSection("SendGridSettings");
        }

        public Task Send(string toAdr, string subject, string msgPlaintxt, string msgHtml)
        {
            var client = new SendGridClient(_config["SendGridKey"]);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("noreply@beep-it.ch", _config["SendGridUser"]),
                Subject = subject,
                PlainTextContent = msgHtml,
                HtmlContent = msgPlaintxt
            };
            msg.AddTo(new EmailAddress(toAdr));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}
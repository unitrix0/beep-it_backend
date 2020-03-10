using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

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

            Response result= client.SendEmailAsync(msg).Result;
            if (result.StatusCode == HttpStatusCode.Accepted) return Task.CompletedTask;

            var sb = new StringBuilder();
            Dictionary<string, dynamic> body = result.DeserializeResponseBodyAsync(result.Body).Result;
            sb.AppendJoin(Environment.NewLine, body.Select(i => $"{i.Key}={i.Value}").ToList());

            Console.WriteLine($"Error Sending Mail: {result.StatusCode} Body: {sb}");
            return Task.CompletedTask;
        }
    }
}
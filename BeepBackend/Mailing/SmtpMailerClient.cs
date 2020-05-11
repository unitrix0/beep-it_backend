using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace BeepBackend.Mailing
{
    public class SmtpMailerClient : IMailerClient
    {
        private IConfigurationSection _config;

        public SmtpMailerClient(IConfiguration config)
        {
            _config = config.GetSection("SmtpMailSettings");
        }

        public Task Send(string toAdr, string subject, string msgTxt)
        {
            var client = new SmtpClient("exch.hive.loc")
            {
                Credentials = new NetworkCredential(_config["User"], _config["Password"], _config["Domain"])
            };
            client.SendCompleted += OnSendCompleted;

            return client.SendMailAsync(new MailMessage()
            {
                From = new MailAddress("postmaster@unimatrix0.ch", "Beep Development"),
                To = { new MailAddress(toAdr) },
                Subject = subject,
                Body = msgTxt,
                IsBodyHtml = true
            });

        }

        private void OnSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine(e.Error != null
                ? $"Error Sending Mail: {e.Error.Message}"
                : "Message sent.");
        }
    }
}
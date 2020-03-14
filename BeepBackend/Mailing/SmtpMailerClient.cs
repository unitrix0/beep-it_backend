using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BeepBackend.Mailing
{
    public class SmtpMailerClient : IMailerClient
    {
        public Task Send(string toAdr, string subject, string msgTxt)
        {
            var client = new SmtpClient("exch.hive.loc")
            {
                Credentials = new NetworkCredential(***REMOVED***)
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
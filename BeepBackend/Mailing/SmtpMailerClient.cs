using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BeepBackend.Mailing
{
    public class SmtpMailerClient : IMailerClient
    {
        public Task Send(string toAdr, string subject, string msgPlaintxt, string msgHtml)
        {
            var client = new SmtpClient("exch.hive.loc")
            {
                Credentials = new NetworkCredential(***REMOVED***)
            };

            return client.SendMailAsync(new MailMessage()
            {
                From = new MailAddress("postmaster@unimatrix0.ch", "Beep Development"),
                To = { new MailAddress(toAdr) },
                Subject = subject,
                Body = msgPlaintxt
            });

        }
    }
}
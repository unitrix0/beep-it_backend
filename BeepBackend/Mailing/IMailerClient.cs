using System.Threading.Tasks;

namespace BeepBackend.Mailing
{
    public interface IMailerClient
    {
        /// <summary>
        /// Sendet die übergeben Nachricht
        /// </summary>
        /// <param name="toAdr">Empfänger Adresse</param>
        /// <param name="subject">Betreff</param>
        /// <param name="msgPlaintxt">Nachricht in Text</param>
        /// <param name="msgHtml">Nachricht in HTML</param>
        /// <returns></returns>
        Task Send(string toAdr, string subject, string msgPlaintxt, string msgHtml);
    }
}
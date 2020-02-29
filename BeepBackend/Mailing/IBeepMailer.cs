using System.Threading.Tasks;
using BeepBackend.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BeepBackend.Mailing
{
    public interface IBeepMailer : IEmailSender
    {
        /// <summary>
        /// Sendet ein Mail an den angegebenen User mit einem Lik zur bestätigung der E-Mail Adresse
        /// </summary>
        /// <param name="userId">Id des Benutzers den die Änderung betrifft</param>
        /// <param name="email">Adresse an die das Mail gesendet werden soll</param>
        /// <param name="token">Token</param>
        /// <param name="isChange">
        ///     <para><c>True</c>: Wenn es sich um eine änderung der E-Mail Adresse handelt.</para>
        ///     <para><c>False</c>: Wenn es sich um eine Bestätigung für einen neuen Account handelt</para></param>
        /// <returns></returns>
        Task SendConfirmationMail(int userId, string email, string token, bool isChange);
    }
}
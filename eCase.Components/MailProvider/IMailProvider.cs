using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eCase.Components.MailProvider
{
    /// <summary>
    /// Интерфейс на компонента за изпращане на имейл
    /// </summary>
    public interface IMailProvider
    {
        void Send(
            string recipient,
            string subject,
            string body,
            bool isBodyHtml,
            IEnumerable<Tuple<string, byte[]>> attachments,
            string senderMail,
            string senderName,
            string bccMail,
            bool hasBccSender);

        Task SendAsync(
            string recipient,
            string subject,
            string body,
            bool isBodyHtml,
            IEnumerable<Tuple<string, byte[]>> attachments,
            string senderMail,
            string senderName,
            string bccMail,
            bool hasBccSender);
    }
}

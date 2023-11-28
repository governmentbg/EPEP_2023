using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace eCase.Components.MailProvider
{
    /// <summary>
    /// Имплементация на компонента за изпращане на имейл
    /// </summary>
    public class VSSProviderImpl : IMailProvider, IDisposable
    {
        #region Public

        /// <summary>
        /// Конструктор на имплементацията
        /// </summary>
        public VSSProviderImpl()
        {
            _smtpClient = new SmtpClient();
            _smtpClient.Host = ConfigurationManager.AppSettings["eCase.Components.MailProvider:MailServerHost"];
            _smtpClient.Port = int.Parse(ConfigurationManager.AppSettings["eCase.Components.MailProvider:MailServerPort"]);

            if (ConfigurationManager.AppSettings["eCase.Components.MailProvider:IOTEST"] != "true")
            {

                _smtpClient.EnableSsl = true;
                _smtpClient.UseDefaultCredentials = true;
                _smtpClient.Credentials = new System.Net.NetworkCredential("noreply@justice.bg", "!!!secret!!!");
            }
            _disposed = false;
        }

        /// <summary>
        /// Изпращане на имейл
        /// </summary>
        /// <param name="recipient">получател</param>
        /// <param name="subject">заглавие</param>
        /// <param name="body">съдържание</param>
        /// <param name="isBodyHtml">аргумент указващ дали съдържанието е html</param>
        /// <param name="attachments">приложени документи</param>
        /// <param name="senderMail">имейл на подателя</param>
        /// <param name="senderName">име на подателя</param>
        /// <returns>true ако успещно е изпратен имейла, иначе false </returns>
        public void Send(
            string recipient,
            string subject,
            string body,
            bool isBodyHtml,
            IEnumerable<Tuple<string, byte[]>> attachments,
            string senderMail,
            string senderName,
            string bccMail,
            bool hasBccSender)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                MailAddress from = new MailAddress(senderMail);
                if (hasBccSender)
                {
                    MailAddress bcc = new MailAddress(bccMail);
                    mailMessage.Bcc.Add(bcc);
                }
                mailMessage.From = from;
                mailMessage.Sender = from;
                mailMessage.To.Add(recipient);
                mailMessage.Subject = subject;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.Item2), attachment.Item1));
                    }
                }

                _smtpClient.Send(mailMessage);
            }
        }

        public async Task SendAsync(
            string recipient,
            string subject,
            string body,
            bool isBodyHtml,
            IEnumerable<Tuple<string, byte[]>> attachments,
            string senderMail,
            string senderName,
            string bccMail,
            bool hasBccSender)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                MailAddress from = new MailAddress(senderMail);
                if (hasBccSender)
                {
                    MailAddress bcc = new MailAddress(bccMail);
                    mailMessage.Bcc.Add(bcc);
                }
                mailMessage.From = from;
                mailMessage.Sender = from;
                mailMessage.To.Add(recipient);
                mailMessage.Subject = subject;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.Item2), attachment.Item1));
                    }
                }

                await _smtpClient.SendMailAsync(mailMessage);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Protected

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_smtpClient != null)
                        _smtpClient.Dispose();
                }

                _smtpClient = null;
                _disposed = true;
            }
        }

        #endregion

        #region Private

        SmtpClient _smtpClient;
        private bool _disposed;

        #endregion
    }
}

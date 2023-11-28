using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace eCase.Components.MailProvider
{
    /// <summary>
    /// Имплементация на компонента за изпращане на имейл
    /// </summary>
    public class GMailProviderImpl : IMailProvider
    {
        public GMailProviderImpl() { }

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
            using (SmtpClient smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ecasebg@gmail.com", "ecasebg2011")
            })
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    MailAddress from = new MailAddress(senderMail, senderName);
                    if (hasBccSender)
                    {
                        MailAddress bcc = new MailAddress(bccMail);
                        mailMessage.Bcc.Add(bcc);
                    }
                    mailMessage.From = from;
                    mailMessage.To.Add(recipient);
                    mailMessage.Subject = subject;
                    mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = isBodyHtml;
                    mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    if (attachments != null)
                        foreach (var attachment in attachments)
                        {
                            mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.Item2), attachment.Item1));
                        }

                    smtpClient.Send(mailMessage);
                }
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
            using (SmtpClient smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ecasebg@gmail.com", "ecasebg2011")
            })
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    MailAddress from = new MailAddress(senderMail, senderName);
                    if (hasBccSender)
                    {
                        MailAddress bcc = new MailAddress(bccMail);
                        mailMessage.Bcc.Add(bcc);
                    }
                    mailMessage.From = from;
                    mailMessage.To.Add(recipient);
                    mailMessage.Subject = subject;
                    mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = isBodyHtml;
                    mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    if (attachments != null)
                        foreach (var attachment in attachments)
                        {
                            mailMessage.Attachments.Add(new Attachment(new MemoryStream(attachment.Item2),
                                attachment.Item1));
                        }

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }
    }
}

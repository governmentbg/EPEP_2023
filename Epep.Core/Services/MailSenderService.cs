using Epep.Core.Contracts;
using Epep.Core.ViewModels.Common;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Epep.Core.Services
{
    public class MailSenderService : IMailSenderService
    {
        private readonly SMTPConfigModel _smtpConfig;
        public MailSenderService(IOptions<SMTPConfigModel> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }

        public string GetFeedbackMail()
        {
            return _smtpConfig.FeedbackEmail;
        }

        public async Task SendEmail(string toEmail, string body, string subject)
        {
            MailMessage mail = new MailMessage
            {
                Subject = subject,
                Body = body,
                From = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
                IsBodyHtml = _smtpConfig.IsBodyHTML
            };

            mail.To.Add(toEmail);

            NetworkCredential networkCredential = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password);

            SmtpClient smtpClient = new SmtpClient
            {
                Host = _smtpConfig.Host,
                Port = _smtpConfig.Port,
                EnableSsl = _smtpConfig.EnableSSL,
                UseDefaultCredentials = _smtpConfig.UseDefaultCredentials,
                Credentials = networkCredential
            };

            mail.BodyEncoding = Encoding.Default;

            await smtpClient.SendMailAsync(mail);
        }
    }
}

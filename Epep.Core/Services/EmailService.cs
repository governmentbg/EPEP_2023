using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Razor.Templating.Core;
using System.Net.Mail;

namespace Epep.Core.Services
{
    public class EmailService : BaseService, IEmailService
    {
        private readonly EmailConfigModel config;
        private readonly IMailSenderService mailService;
        private readonly IRazorTemplateEngine razorTemplateEngine;
        public EmailService(
            IOptions<EmailConfigModel> config,
            IMailSenderService mailService,
            IRazorTemplateEngine razorTemplateEngine,
            ILogger<EmailService> logger,
            IRepository repo)
        {
            this.config = config.Value;
            this.mailService = mailService;
            this.razorTemplateEngine = razorTemplateEngine;
            this.logger = logger;
            this.repo = repo;
        }
        public async Task SendMails()
        {
            var emailIds = await getPendingEmailIds(config.BatchSize, config.MaxFailAttempts, config.AttemptTimeAsTimeSpan);
            foreach (var emailId in emailIds)
            {
                await sendMail(emailId);
            }
        }

        private async Task sendMail(long emailId)
        {
            var email = await repo.GetByIdAsync<Email>(emailId);
            if (email == null || email.Status != EmailStatus.Pending)
            {
                return;
            }

            try
            {
                await mailService.SendEmail(
                    email.Recipient,
                    await buildEmailBody(email.MailTemplateName, email.Context),
                    config.GetEmailSubject(email.MailTemplateName));

                email.Status = EmailStatus.Sent;
                email.ModifyDate = DateTime.Now;
            }
            catch (SmtpException smtpEx)
            {
                var exception = "SmtpException: " + Enum.GetName(typeof(SmtpStatusCode), smtpEx.StatusCode);
                email.IncrementFailedAttempts(exception);
            }
            catch (Exception ex)
            {
                email.Status = EmailStatus.UknownError;
                email.IncrementFailedAttempts(ex.Message);
            }
            await repo.SaveChangesAsync();
        }

        public async Task<string> buildEmailBody(string templateFileName, string context)
        {
            var viewData = new Dictionary<string, object>
            {
                { "portalUrl", config.PortalUrl },
                { "feedbackUrl", config.FeedbackUrl }
            };
            var model = (context != null) ? JObject.Parse(context) : null;
            var html = await razorTemplateEngine.RenderAsync($"~/Views/EmailTemplates/{templateFileName}.cshtml", model, viewData);

            return html;
        }


        async Task<IList<long>> getPendingEmailIds(int limit, int maxFailedAttempts, TimeSpan failedAttemptTimeout)
        {
            var maxInterval = DateTime.Now - failedAttemptTimeout;
            var emails = repo.AllReadonly<Email>().Where(e => e.Status == EmailStatus.Pending);
            return await emails.Where(e => e.FailedAttempts == 0).Union(emails.Where(e => e.FailedAttempts < maxFailedAttempts && e.ModifyDate < maxInterval))
                    .OrderBy(e => e.CreateDate)
                .Select(e => e.EmailId)
                .Take(limit)
                .ToListAsync();
        }

        public async Task NewMailMessage(string emailAddress, string templateName, JObject context, bool autoSave = true)
        {
            Email email = new Email()
            {
                Recipient = emailAddress,
                MailTemplateName = templateName,
                FailedAttempts = 0,
                Status = EmailStatus.Pending,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            if (context != null)
            {
                email.Context = context.ToString();
            }
            await repo.AddAsync(email);
            if (autoSave)
            {
                try
                {
                    await repo.SaveChangesAsync();
                }
                catch (Exception ex) { }
            }
        }


    }
}

using eCase.Components;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace eCase.Web.Jobs
{
    public class TemplateConfig
    {
        public string TemplateName { get; private set; }
        public string TemplateFileName { get; private set; }
        public string SenderMail { get; private set; }
        public string SenderName { get; private set; }
        public string MailSubject { get; private set; }
        public bool IsBodyHtml { get; private set; }

        private static IList<TemplateConfig> AllTemplates;

        private TemplateConfig(string templateName, string templateFileName, string senderMail, string senderName, string mailSubject, bool isBodyHtml)
        {
            this.TemplateName = templateName;
            this.TemplateFileName = templateFileName;
            this.SenderMail = senderMail;
            this.SenderName = senderName;
            this.MailSubject = mailSubject;
            this.IsBodyHtml = isBodyHtml;
        }

        public static TemplateConfig Get(string name)
        {
            return AllTemplates.Single(e => e.TemplateName == name);
        }

        static TemplateConfig()
        {
            AllTemplates = new List<TemplateConfig>()
            {
                new TemplateConfig(
                    EmailTemplates.ForgottenPasswordMessage,
                    EmailTemplates.ForgottenPasswordMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Забравена парола)",
                    true),  
                new TemplateConfig(
                    EmailTemplates.NewRegistrationMessage,
                    EmailTemplates.NewRegistrationMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Регистрация на нов потребител)",
                    true), 
                new TemplateConfig(
                    EmailTemplates.SummonNotificationMessage,
                    EmailTemplates.SummonNotificationMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Съобщение по дело)",
                    true),
                new TemplateConfig(
                    EmailTemplates.SummonAccessActivationMessage,
                    EmailTemplates.SummonAccessActivationMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Електронни призовки и съобщения)",
                    true),
                new TemplateConfig(
                    EmailTemplates.SummonAccessDeactivationMessage,
                    EmailTemplates.SummonAccessDeactivationMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Електронни призовки и съобщения)",
                    true),
                new TemplateConfig(
                    EmailTemplates.FeedbackMessage,
                    EmailTemplates.FeedbackMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Обратна връзка)",
                    true),
                new TemplateConfig(
                    EmailTemplates.CaseAccessMessage,
                    EmailTemplates.CaseAccessMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Нов достъп до дело)",
                    true),
               new TemplateConfig(
                    EmailTemplates.ChangeUserNameMessage,
                    EmailTemplates.ChangeUserNameMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Промяна на потребителско име)",
                    true),
              new TemplateConfig(
                    EmailTemplates.ChangeCaseAccessMessage,
                    EmailTemplates.ChangeCaseAccessMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Промяна на права за достъп до дело)",
                    true),
             new TemplateConfig(
                    EmailTemplates.ChangeUserProfileMessage,
                    EmailTemplates.ChangeUserProfileMessage + ".cshtml",
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderMail"],
                    ConfigurationManager.AppSettings["eCase.Web.MailJob:SenderName"],
                    "Единен портал за електронно правосъдие (Промяна на потребителски профил)",
                    true)
            };
        }
    }
}
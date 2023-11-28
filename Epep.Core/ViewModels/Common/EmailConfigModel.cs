namespace Epep.Core.ViewModels.Common
{
    public class EmailConfigModel
    {
        public string PortalUrl { get; set; }
        public string FeedbackUrl { get; set; }
        public int BatchSize { get; set; }
        public int MaxFailAttempts { get; set; }
        public int FailAttemptTimeout { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }

        public string GetEmailSubject(string templateName)
        {
            switch (templateName)
            {
                case "ForgottenPasswordMessage":
                    return "Единен портал за електронно правосъдие (Забравена парола)";

                case "NewRegistrationMessage":
                    return "Единен портал за електронно правосъдие (Регистрация на нов потребител)";
                case "SummonNotificationMessage":
                    return "Единен портал за електронно правосъдие (Съобщение по дело)";
                case "SummonAccessActivationMessage":
                    return "Единен портал за електронно правосъдие (Електронни призовки и съобщения)";
                case "SummonAccessDeactivationMessage":
                    return "Единен портал за електронно правосъдие (Електронни призовки и съобщения)";
                case "FeedbackMessage":
                    return "Единен портал за електронно правосъдие (Обратна връзка)";
                case "CaseAccessMessage":
                    return "Единен портал за електронно правосъдие (Нов достъп до дело)";
                case "ChangeUserNameMessage":
                    return "Единен портал за електронно правосъдие (Промяна на потребителско име)";
                case "ChangeCaseAccessMessage":
                    return "Единен портал за електронно правосъдие (Промяна на права за достъп до дело)";
                case "ChangeUserProfileMessage":
                    return "Единен портал за електронно правосъдие (Промяна на потребителски профил)";
                default:
                    return "Единен портал за електронно правосъдие";
            }
        }

        public TimeSpan AttemptTimeAsTimeSpan
        {
            get
            {
                return TimeSpan.FromMinutes(this.FailAttemptTimeout);
            }
        }
    }
}
namespace Epep.Core.Contracts
{
    public interface IMailSenderService
    {
        string GetFeedbackMail();
        Task SendEmail(string toEmail, string body, string subject);
    }
}
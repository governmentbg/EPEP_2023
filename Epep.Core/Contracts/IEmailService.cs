using Newtonsoft.Json.Linq;

namespace Epep.Core.Contracts
{
    public interface IEmailService : IBaseService
    {
        Task<string> buildEmailBody(string templateFileName, string context);
        Task NewMailMessage(string emailAddress, string templateName, JObject context, bool autoSave = true);
        Task SendMails();
    }
}

using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class SummonNotificationHandler : EventHandler<SummonNotificationEvent>
    {
        private IMailRepository mailRepository;

        public SummonNotificationHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(SummonNotificationEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.SummonNotificationMessage,
                JObject.FromObject(
                    new
                    {
                        email = e.Email
                    }));

            this.mailRepository.Add(email);
        }
    }
}

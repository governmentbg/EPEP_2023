using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class FeedbackHandler : EventHandler<FeedbackEvent>
    {
        private IMailRepository mailRepository;

        public FeedbackHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(FeedbackEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.FeedbackMessage,
                JObject.FromObject(
                    new
                    {
                        type = e.Type,
                        name = e.Name,
                        email = e.Email,
                        message = e.Message
                    }));

            this.mailRepository.Add(email);
        }
    }
}

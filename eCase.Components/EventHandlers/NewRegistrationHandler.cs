using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class NewRegistrationHandler : EventHandler<NewRegistrationEvent>
    {
        private IMailRepository mailRepository;

        public NewRegistrationHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(NewRegistrationEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.NewRegistrationMessage,
                JObject.FromObject(
                    new
                    {
                        activationCode = e.ActivationCode,
                        email = e.Email
                    }));

            this.mailRepository.Add(email);
        }
    }
}

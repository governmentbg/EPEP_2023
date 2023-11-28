using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class ForgottenPasswordHandler : EventHandler<ForgottenPasswordEvent>
    {
        private IMailRepository mailRepository;

        public ForgottenPasswordHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(ForgottenPasswordEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.ForgottenPasswordMessage,
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

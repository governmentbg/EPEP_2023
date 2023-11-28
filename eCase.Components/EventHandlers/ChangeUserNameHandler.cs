using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class ChangeUserNameHandler : EventHandler<ChangeUserNameEvent>
    {
        private IMailRepository mailRepository;

        public ChangeUserNameHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(ChangeUserNameEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.ChangeUserNameMessage,
                JObject.FromObject(
                    new
                    {
                        activationCode = e.ActivationCode,
                        email = e.Email,
                        courtName = e.CourtName,
                        oldUserName = e.OldUserName
                    }));

            this.mailRepository.Add(email);
        }
    }
}

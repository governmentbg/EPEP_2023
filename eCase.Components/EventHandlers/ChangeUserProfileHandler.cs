using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class ChangeUserProfileHandler : EventHandler<ChangeUserProfileEvent>
    {
        private IMailRepository mailRepository;

        public ChangeUserProfileHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(ChangeUserProfileEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.ChangeUserProfileMessage,
                JObject.FromObject(
                    new
                    {
                        isActivated = e.IsActivated,
                        email = e.Email,
                        courtName = e.CourtName,
                        name = e.Name
                    }));

            this.mailRepository.Add(email);
        }
    }
}

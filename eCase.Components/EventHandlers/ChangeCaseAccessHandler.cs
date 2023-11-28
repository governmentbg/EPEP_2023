using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class ChangeCaseAccessHandler : EventHandler<ChangeCaseAccessEvent>
    {
        private IMailRepository mailRepository;

        public ChangeCaseAccessHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(ChangeCaseAccessEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.ChangeCaseAccessMessage,
                JObject.FromObject(
                    new
                    {
                        caseAbbr = e.CaseAbbr,
                        courtName = e.CourtName,
                        email = e.Email
                    }));

            this.mailRepository.Add(email);
        }
    }
}

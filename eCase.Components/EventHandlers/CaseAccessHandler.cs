using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class CaseAccessHandler : EventHandler<CaseAccessEvent>
    {
        private IMailRepository mailRepository;

        public CaseAccessHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(CaseAccessEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.CaseAccessMessage,
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

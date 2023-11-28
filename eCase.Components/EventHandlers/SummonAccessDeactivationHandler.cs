using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class SummonAccessDeactivationHandler : EventHandler<SummonAccessDeactivationEvent>
    {
        private IMailRepository mailRepository;

        public SummonAccessDeactivationHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(SummonAccessDeactivationEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.SummonAccessDeactivationMessage,
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

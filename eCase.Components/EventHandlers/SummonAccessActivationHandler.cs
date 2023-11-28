using Newtonsoft.Json.Linq;

using eCase.Data.Repositories;
using eCase.Domain.Core;
using eCase.Domain.Emails;
using eCase.Domain.Events;

namespace eCase.Components.EventHandlers
{
    public class SummonAccessActivationHandler : EventHandler<SummonAccessActivationEvent>
    {
        private IMailRepository mailRepository;

        public SummonAccessActivationHandler(IMailRepository mailRepository)
        {
            this.mailRepository = mailRepository;
        }

        public override void Handle(SummonAccessActivationEvent e)
        {
            Email email = new Email(
                e.Email,
                EmailTemplates.SummonAccessActivationMessage,
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

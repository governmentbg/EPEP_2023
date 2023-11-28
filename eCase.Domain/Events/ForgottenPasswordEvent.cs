using eCase.Domain.Core;

namespace eCase.Domain.Events
{
    public class ForgottenPasswordEvent : IDomainEvent
    {
        public string Email { get; set; }

        public string ActivationCode { get; set; }
    }
}

using eCase.Domain.Core;

namespace eCase.Domain.Events
{
    public class NewRegistrationEvent : IDomainEvent
    {
        public string Email { get; set; }

        public string ActivationCode { get; set; }
    }
}
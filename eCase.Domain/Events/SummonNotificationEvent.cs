using eCase.Domain.Core;

namespace eCase.Domain.Events
{
    public class SummonNotificationEvent : IDomainEvent
    {
        public string Email { get; set; }
    }
}
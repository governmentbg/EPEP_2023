using eCase.Domain.Core;

namespace eCase.Domain.Events
{
    public class FeedbackEvent : IDomainEvent
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }
    }
}

using eCase.Domain.Core;

namespace eCase.Domain.Events
{
    public class ChangeUserProfileEvent : IDomainEvent
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string IsActivated { get; set; }

        public string CourtName { get; set; }
    }
}

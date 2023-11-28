using eCase.Domain.Core;

namespace eCase.Domain.Events
{
    public class ChangeUserNameEvent : IDomainEvent
    {
        public string OldUserName { get; set; }

        public string Email { get; set; }

        public string ActivationCode { get; set; }

        public string CourtName { get; set; }
    }
}

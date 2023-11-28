﻿using eCase.Domain.Core;

namespace eCase.Domain.Events
{
    public class SummonAccessActivationEvent : IDomainEvent
    {
        public string CaseAbbr { get; set; }

        public string CourtName { get; set; }

        public string Email { get; set; }
    }
}

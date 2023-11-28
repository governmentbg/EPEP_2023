using System.Collections.Generic;

namespace eCase.Domain.Core
{
    public interface IEventEmitter
    {
        ICollection<IDomainEvent> Events { get; set; }
    }
}

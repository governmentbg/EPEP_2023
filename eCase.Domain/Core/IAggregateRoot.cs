using System;

namespace eCase.Domain.Core
{
    public interface IAggregateRoot
    {
        byte[] Version { get; set; }

        DateTime CreateDate { get; set; }

        DateTime ModifyDate { get; set; }
    }
}

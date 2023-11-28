namespace Epep.Core.Models
{
    public interface IAggregateRoot : IGidRoot
    {
        byte[] Version { get; set; }

        DateTime CreateDate { get; set; }

        DateTime ModifyDate { get; set; }
    }
}

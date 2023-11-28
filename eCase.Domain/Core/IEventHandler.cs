namespace eCase.Domain.Core
{
    public interface IEventHandler
    {
        void Handle(IDomainEvent e);
    }
}

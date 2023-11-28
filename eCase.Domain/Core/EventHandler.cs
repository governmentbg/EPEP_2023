
namespace eCase.Domain.Core
{
    public abstract class EventHandler<TEvent> : IEventHandler where TEvent : IDomainEvent
    {
        public void Handle(IDomainEvent e)
        {
            if (e is TEvent)
            {
                this.Handle((TEvent)e);
            }
        }

        public abstract void Handle(TEvent e);
    }
}

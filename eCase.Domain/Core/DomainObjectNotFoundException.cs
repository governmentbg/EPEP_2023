namespace eCase.Domain.Core
{
    public class DomainObjectNotFoundException : DomainException
    {
        public DomainObjectNotFoundException(string message)
            : base(message)
        {
        }
    }
}

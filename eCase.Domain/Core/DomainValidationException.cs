namespace eCase.Domain.Core
{
    public class DomainValidationException : DomainException
    {
        public DomainValidationException(string message)
            : base(message)
        {
        }
    }
}

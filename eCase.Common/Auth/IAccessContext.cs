namespace eCase.Common.Auth
{
    public interface IAccessContext
    {
        bool IsAuthenticated { get; }
        bool IsUser { get; }
        bool IsRegistration { get; }
        bool IsContractRegistration { get; }
        bool IsContractAccessCode { get; }
        int UserId { get; }
        int RegistrationId { get; }
        int ContractRegistrationId { get; }
        int ContractAccessCodeId { get; }
    }
}

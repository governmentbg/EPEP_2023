using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.Contracts
{
    public interface IUserContext
    {
        /// <summary>
        /// Идентификатор на потребител
        /// </summary>
        long UserId { get; }

        /// <summary>
        /// Идентификатор на организация към потребител
        /// </summary>
        long? OrganizationUserId { get; }

        /// <summary>
        /// Имена
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Вид поведение на потребител
        /// </summary>
        int UserType { get; }

        /// <summary>
        /// Идентификатор на лице
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// Флаг на автентикиран потребител
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// За представляващи и юристи към организация -  Имена на организация
        /// </summary>
        string OrganizationName { get; }
        string Email { get; }
        long CourtId { get; }
        string OrganizationUIC { get; }
        string CertificateNumber { get; }
        string UserTypeName { get; }
        long AccessUserId { get; }
        string RegCertInfo { get; }
        string UserTypeNameClass { get; }
    }
}

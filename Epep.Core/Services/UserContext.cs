using Epep.Core.Constants;
using Epep.Core.Contracts;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Security.Claims;

namespace Epep.Core.Services
{
    public class UserContext : IUserContext
    {
        private ClaimsPrincipal User;

        public UserContext(IHttpContextAccessor _ca)
        {
            User = _ca.HttpContext.User;
        }

        public bool IsAuthenticated => User != null && User.Identity.IsAuthenticated && this.UserId > 0;
        public long UserId
        {
            get
            {
                long userId = 0;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {
                    var subClaim = User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.Sid);

                    if (subClaim != null)
                    {
                        userId = long.Parse(subClaim.Value);
                    }
                }

                return userId;
            }
        }
        public long? OrganizationUserId
        {
            get
            {
                long? userId = null;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {
                    var subClaim = User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid);

                    if (subClaim != null)
                    {
                        userId = long.Parse(subClaim.Value);
                    }
                }

                return userId;
            }
        }

        public long AccessUserId
        {
            get
            {
                switch (this.UserType)
                {
                    case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    case NomenclatureConstants.UserTypes.OrganizationUser:
                        return this.OrganizationUserId ?? 0;
                    default:
                        return this.UserId;
                }
            }
        }
        public int UserType
        {
            get
            {
                int result = 0;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {
                    var subClaim = User.Claims
                        .FirstOrDefault(c => c.Type == CustomClaimTypes.UserType);

                    if (subClaim != null)
                    {
                        result = int.Parse(subClaim.Value);
                    }
                }

                return result;
            }
        }
        public long CourtId
        {
            get
            {
                long result = 0;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {
                    var subClaim = User.Claims
                        .FirstOrDefault(c => c.Type == CustomClaimTypes.Court);

                    if (subClaim != null)
                    {
                        result = long.Parse(subClaim.Value);
                    }
                }

                return result;
            }
        }
        public string Identifier
        {
            get
            {
                string result = String.Empty;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {

                    var claimEmail = User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                    if (claimEmail != null)
                        result = claimEmail.Value;
                }

                return result;
            }
        }

        public string OrganizationUIC
        {
            get
            {
                string result = String.Empty;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {

                    var claimEmail = User.Claims
                        .FirstOrDefault(c => c.Type == CustomClaimTypes.OrganizationUic);

                    if (claimEmail != null)
                        result = claimEmail.Value;
                }

                return result;
            }
        }

        public string RegCertInfo
        {
            get
            {
                string result = String.Empty;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {

                    var claimEmail = User.Claims
                        .FirstOrDefault(c => c.Type == CustomClaimTypes.RegCertInfo);

                    if (claimEmail != null)
                        result = claimEmail.Value;
                }

                return result;
            }
        }


        public string FullName
        {
            get
            {
                string fullName = String.Empty;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {
                    var subClaim = User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.Name);

                    if (subClaim != null)
                    {
                        fullName = subClaim.Value;
                    }
                }

                return fullName;
            }
        }

        public string Email
        {
            get
            {
                string value = String.Empty;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {
                    var subClaim = User.Claims
                        .FirstOrDefault(c => c.Type == ClaimTypes.Email);

                    if (subClaim != null)
                    {
                        value = subClaim.Value;
                    }
                }

                return value;
            }
        }

        public string OrganizationName
        {
            get
            {
                string orgName = String.Empty;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {
                    var subClaim = User.Claims
                        .FirstOrDefault(c => c.Type == CustomClaimTypes.OrganizationName);

                    if (subClaim != null)
                    {
                        orgName = subClaim.Value;
                    }
                }

                return orgName;
            }
        }

        public string CertificateNumber
        {
            get
            {
                string certNo = string.Empty;

                if (User != null && User.Claims != null && User.Claims.Count() > 0)
                {

                    var claimCertNo = User.Claims
                        .FirstOrDefault(c => c.Type == CustomClaimTypes.IdStampit.CertificateNumber);

                    certNo = claimCertNo?.Value;
                }

                return certNo;
            }
        }

        public string UserTypeName
        {
            get
            {
                switch (this.UserType)
                {
                    case NomenclatureConstants.UserTypes.Person:
                        return "Физическо лице";
                    case NomenclatureConstants.UserTypes.Lawyer:
                        return "Адвокат";
                    case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                        return "Представляващ ЮЛ";
                    case NomenclatureConstants.UserTypes.OrganizationUser:
                        return "Юрисконсулт";
                    case NomenclatureConstants.UserTypes.GlobalAdmin:
                        return "Администратор";
                    case NomenclatureConstants.UserTypes.CourtAdmin:
                        return "Администратор на съд";
                    default:
                        return "";
                }
            }
        }
        public string UserTypeNameClass
        {
            get
            {
                switch (this.UserType)
                {
                    case NomenclatureConstants.UserTypes.Person:
                        return "page-header__role--person";
                    case NomenclatureConstants.UserTypes.Lawyer:
                        return "page-header__role--legal-advisor";
                    case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                        return "page-header__role--representative";
                    case NomenclatureConstants.UserTypes.OrganizationUser:
                        return "page-header__role--representative";
                    case NomenclatureConstants.UserTypes.GlobalAdmin:
                        return "page-header__role--administrator";
                    case NomenclatureConstants.UserTypes.CourtAdmin:
                        return "page-header__role--court-admin";
                    default:
                        return "";
                }
            }
        }
    }
}


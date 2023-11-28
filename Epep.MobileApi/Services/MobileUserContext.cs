using Epep.Core.Constants;
using Epep.Core.Contracts;

namespace Epep.MobileApi.Services
{
    public class MobileUserContext : IUserContext
    {
        public long UserId => 86;

        public long? OrganizationUserId => throw new NotImplementedException();

        public string FullName => "Test mobile user";

        public int UserType => NomenclatureConstants.UserTypes.Person;

        public string Identifier => throw new NotImplementedException();

        public bool IsAuthenticated => true;

        public string OrganizationName => throw new NotImplementedException();

        public string Email => throw new NotImplementedException();

        public long CourtId => 0;

        public string OrganizationUIC => throw new NotImplementedException();

        public string CertificateNumber => throw new NotImplementedException();

        public string UserTypeName => throw new NotImplementedException();

        public long AccessUserId => throw new NotImplementedException();

        public string RegCertInfo => throw new NotImplementedException();

        public string UserTypeNameClass => throw new NotImplementedException();
    }
}

namespace Epep.Core.Constants
{
    public static class CustomClaimTypes
    {
        public static class IdStampit
        {
            public static string PersonalId = "urn:stampit:pid";

            public static string Organization = "urn:stampit:organization";

            public static string PublicKey = "urn:stampit:public_key";

            public static string CertificateNumber = "urn:stampit:certno";
        }

        public static string FullName = "urn:io:full_name";
        public static string UIC = "urn:io:uic";
        public static string UserType = "urn:io:usertype";
        public static string Court = "urn:io:court";
        public static string OrganizationUic = "urn:io:orguic";
        public static string OrganizationName = "urn:io:orgname";
        public static string RegCertInfo = "urn:io:regcert";
    }
}

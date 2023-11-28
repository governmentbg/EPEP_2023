namespace eCase.Domain.Service.Entities.Upgrade
{
    public class EpepConstants
    {
        public class UserTypes
        {
            public const int Person = 1;
            public const int Organization = 3;

            public static int[] eCaseUserTypes = { Person, Organization };
        }

        public class UserAssignmentRoles
        {
            public const int Lawyer = 1;
            public const int Side = 2;

            public static int?[] AcceptedRoles = { Lawyer, Side };

        }

        public static class AttachedTypes
        {
            public const int IncommingDocument = 1;
            public const int OutgoingDocument = 2;
            public const int ActCoordination = 3;
            public const int ActCoordinationPublic = 4;
            public const int ActFiles = 41;
            public const int SessionFastDocument = 5;
            public const int UserRegistration = 6;
            public const int UserRegistrationRegixReport = 61;
            public const int ElectronicDocument = 7;
            public const int ElectronicDocumentMain = 71;
            public const int ElectronicDocumentTimestamp = 72;
            public const int ElectronicPayment = 8;
            public const int MoneyObligation = 8;

            public const int SignTempFile = 80;
        }

    }
}

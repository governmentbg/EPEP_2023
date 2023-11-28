namespace Epep.Core.Constants
{
    public class NomenclatureConstants
    {
        public const string NormalDateFormat = "dd.MM.yyyy";

        public const string CyliricLetersAndNumbers = @"^([а-яА-Я0-9 \-,.]+)$";
        public const string EmailRegexPattern = @"^(?:[a-zA-Z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+\/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[A-Za-z0-9-]*[A-Za-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$";
        public class Messages
        {
            public const string IsRequired = "Полето е задължително.";
            public const string InvalidValue = "Невалидна стойност.";
            public const string InvalidOperation = "Непозволена операция.";
            public const string NotFound = "Ненамерен обект.";
        }
        public class FocusTypes
        {
            /// <summary>
            /// Дела на фокус
            /// </summary>
            public const int Focus = -1;

            /// <summary>
            /// Архивирани дела
            /// </summary>
            public const int Archive = 1;

            /// <summary>
            /// Достъпени дела
            /// </summary>
            public const int View = 10;

            /// <summary>
            /// Достъпени дела по закона за адвокатурата
            /// </summary>
            public const int LawyerView = 11;
        }

        public class AuditOperations
        {
            public const int View = 1;
            public const int Append = 2;
            public const int Update = 3;
            public const int FileDownload = 4;
            public const int Patch = 5;
            public const int ComfirmUL = 6;
            public const int DenyUL = 7;
        }
        public class LoginProperties
        {
            public const string StampItProvider = "IdStampIT";
            public const string EntityType = "etype";
            public const string Uic = "uic";
        }

        public class UserTypes
        {
            public const int Person = 1;
            public const int Lawyer = 2;

            public const int Organization = 3;
            public const int OrganizationRepresentative = 4;
            public const int OrganizationUser = 5;
            public static int[] OrganizationTypes = { Organization, OrganizationRepresentative, OrganizationUser };
            public static int[] OrganizationUserTypes = { OrganizationRepresentative, OrganizationUser };

            public const int Administrator = 8;
            public const int CourtAdmin = 9;
            public const int GlobalAdmin = 10;
            public static int[] AdministratorTypes = { Administrator, CourtAdmin, GlobalAdmin };
            public static int[] PersonTypes = { Person, Lawyer, OrganizationRepresentative, OrganizationUser };
            public static int[] PublicTypes = { Person, Lawyer, Organization };
            public static int[] CanRequestAccess = { Person, Lawyer, OrganizationRepresentative };
            public static int[] LogCaseViewTypes = { Person, Lawyer, OrganizationRepresentative, OrganizationUser };
            public static int[] AdminTypes = { Administrator };
            public static int[] SideAccessTypes = { Lawyer, CourtAdmin, GlobalAdmin };
            public const int Registration = 99;
        }

        public class LawyerTypes
        {
            /// <summary>
            /// Адвокат
            /// </summary>
            public const long Lawyer = 1;

            /// <summary>
            /// Чуждестранен адвокат
            /// </summary>
            public const long ForeignLawyer = 3;

            /// <summary>
            /// Младши адвокат
            /// </summary>
            public const long JuniorLawyer = 6;

            public static long[] LawyerLoginTypes = { Lawyer, ForeignLawyer, JuniorLawyer };

        }

        public class LawyerStates
        {
            public const long Active = 1;
            public const long TemporarySusspended = 2;
            public const long Deactivated = 3;
            public const long RemovedFromCollage = 4;

            public static long?[] LoginStates = { Active, TemporarySusspended };
            public static long[] AcceptedStates = { Active, TemporarySusspended, Deactivated, RemovedFromCollage };
        }

        public class UserAssignmentRoles
        {
            /// <summary>
            /// Адвокат
            /// </summary>
            public const int Lawyer = 1;

            /// <summary>
            /// Страна - Физическо лице
            /// </summary>
            public const int Side = 2;
        }

        public class UserAccessModes
        {

            public const int RequestAccess = 10;
            public const int RemoveAccess = 11;

            public const int RequestSummon = 20;
            public const int RemoveSummon = 21;
        }

        public class SideInvolmentTypes
        {
            public const int Vnositel = 1;
            public const int Vazlojitel = 2;
        }

        public class SideTypes
        {
            public const int LeftInit = 1;
            public const int Right = 2;
            public const int Other = 3;
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

        public class SourceTypes
        {
            public const int IncommingDocument = 101;
            public const int OutgoingDocument = 102;
            public const int Case = 2;
            public const int Assignment = 3;
            public const int Side = 4;
            public const int Hearing = 5;
            public const int HearingDocument = 501;
            public const int Summon = 6;
            public const int SummonReport = 61;
            public const int SummonReadStamp = 62;
            public const int Act = 7;
            public const int ActPublic = 701;
            public const int ActPrivate = 702;
            public const int MotivePublic = 703;
            public const int MotivePrivate = 704;
            public const int CaseScannedFiles = 80;
        }

        public class SubjectTypes
        {
            public const int Person = 1;
            public const int Entity = 2;
        }

        public class MoneyValueTypes
        {
            public const int Value = 1;
            public const int Procent = 2;
        }
        public class Currencies
        {
            public const int BGN = 1;
            public const int EUR = 2;
        }

        public class MoneyObligationTypes
        {
            public const int CourtTax = 1;
            public const int Obligation = 2;
        }

        public class MoneyAccountType
        {
            /// <summary>
            /// Депозитна сметка - глоби и разноски
            /// </summary>
            public const int Deposit = 1;

            /// <summary>
            /// Бюджетна сметка - съдебни такси по документи
            /// </summary>
            public const int Budget = 2;
        }

        public const int UserVacation_MaxYearDays = 60;

        public class DocumentKinds
        {
            public const int Initial = 1;
            public const int Compliant = 2;
            public const int SideDoc = 3;
        }

        public const string InitDocumentSummonConsent = "ВАЖНО: С подаването на този документ се съгласявам, че мога да бъда известяван(а) през ЕПЕП за съобщения и призовки в хода на делото.";

        public class CourtTypes
        {
            public const int Rayonen = 2;
        }

        public class UserVacationTypes
        {
            public const int Vacation = 1;
            public const int Ill = 2;
        }

        public class EmailTemplates
        {
            public const string NewUserRegistrationMessage = "NewUserRegistrationMessage";
            public const string NewRegistrationMessage = "NewRegistrationMessage";
            public const string ChangeUserProfileMessage = "ChangeUserProfileMessage";
            public const string ApprovalRegistrationMessage = "ApprovalRegistrationMessage";
            public const string CaseAccessMessage = "CaseAccessMessage";
            public const string ChangeCaseAccessMessage = "ChangeCaseAccessMessage";
            public const string SummonNotificationMessage = "SummonNotificationMessage";
            public const string SummonAccessActivationMessage = "SummonAccessActivationMessage";
            public const string SummonAccessDeactivationMessage = "SummonAccessDeactivationMessage";
            public const string FeedbackMessage = "FeedbackMessage";
            public const string DocumentCleanMessage = "DocumentCleanMessage";
        }

        public class CodeMappingAlias
        {
            public const string LawyerTypes = "lawyer_types";
            public const string LawyerStates = "lawyer_states";
            public const string LawyerCollages = "lawyer_collages";
        }


        public class FilesAccepted
        {
            public const string PDF = "pdf";
            public const string PDFsigned = "pdf.p7s";
            public const string P7S = "p7s";
            public const string DOC = "doc.p7s";
            public const string DOCX = "docx.p7s";
            public const string XLS = "xls.p7s";
            public const string XLSX = "xlsx.p7s";
            public const string JPG = "jpg.p7s";
            public const string JPEG = "jpeg.p7s";
            public const string PNG = "png.p7s";
            public const string TIFF = "tiff.p7s";
            public const string GIF = "gif.p7s";

            public static string[] AcceptedExt = { PDF, P7S };
            public static string[] AcceptedFileEnds = { PDF, PDFsigned, DOC, DOCX, XLS, XLSX, JPG, JPEG, PNG, TIFF, GIF };
        }

        public class SystemCodes
        {
            public const short EISS = 1;
        }
    }
}

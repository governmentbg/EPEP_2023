using System.Linq;

namespace Epep.Core.ViewModels.Regix
{
    public static class RegixExtensions
    {
        #region НБД

        public static string MakeFullName(this IO.RegixClient.PersonNames model)
        {
            string result = model.FirstName;
            if (!string.IsNullOrEmpty(model.SurName))
            {
                result += " " + model.SurName;
            }
            if (!string.IsNullOrEmpty(model.FamilyName))
            {
                result += " " + model.FamilyName;
            }

            return result;
        }

        #endregion

        #region Търговски регистър

        protected static class TRSectionMap
        {
            /// <summary>
            /// Основни данни
            /// </summary>
            public const string Section_MainData = "Item1";

            /// <summary>
            /// Седалище и адрес на управление
            /// </summary>
            public const string Record_CompanyName = "00020";

            /// <summary>
            /// Седалище и адрес на управление
            /// </summary>
            public const string Record_HeadQuatersAddress = "00050";

            /// <summary>
            /// Съвет на директорите
            /// </summary>
            public const string Record_LegalOrganManagers = "001201";

            /// <summary>
            /// Едноличен собственик на капитала
            /// </summary>
            public const string Record_LegalOrgan = "00230";

            /// <summary>
            /// Предмет на дейност
            /// </summary>
            public const string Record_MainActivity = "00060";

            /// <summary>
            /// Размер на капитала
            /// </summary>
            public const string Record_Capital = "00310";

            /// <summary>
            /// Представители
            /// </summary>
            public const string Record_Representatives = "001000";

            /// <summary>
            /// Начин на представляване
            /// </summary>
            public const string Record_RepresentType = "00110";
            public const string Record_Managers = "000700";
            public const string Record_CoOwners = "001900";
            public const string Record_ManagersDeleg = "00071";


        }
        public static void ToOrganization(this IO.RegixClient.Models.RegixData.RegixActualStateV3ResponseVM data, EntityInfoVM organization)
        {
            organization.LegalForm = data.LegalForm;
            organization.Label = getDeedRecord(data, TRSectionMap.Section_MainData, TRSectionMap.Record_CompanyName);
            organization.HeadquatersAddress = getDeedRecord(data, TRSectionMap.Section_MainData, TRSectionMap.Record_HeadQuatersAddress);
            organization.LegalOrgan = getDeedRecord(data, TRSectionMap.Section_MainData, TRSectionMap.Record_LegalOrgan);
            organization.LegalOrganManagers = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, TRSectionMap.Record_LegalOrganManagers);
            organization.MainActivity = getDeedRecord(data, TRSectionMap.Section_MainData, TRSectionMap.Record_MainActivity);

            organization.Representatives = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, TRSectionMap.Record_Representatives);
            organization.RepresentType = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, TRSectionMap.Record_RepresentType);
            organization.Managers = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, TRSectionMap.Record_Managers);
            organization.CoOwners = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, TRSectionMap.Record_CoOwners);

            //var p100 = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, "00100");
            //var p101 = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, "00101");
            //var p102 = getDeedRecordFromArray(data, TRSectionMap.Section_MainData, "00102");
            //var preds = getDeedRecord(data, TRSectionMap.Section_MainData, "00110"); 
            //organization.Capital = getDeedRecord(data, TRSectionMap.Section_MainData, TRSectionMap.Record_Capital).ToDecimalSafe();
        }

        private static string getDeedRecord(this IO.RegixClient.Models.RegixData.RegixActualStateV3ResponseVM data, string sectionCode, string recordCode)
        {
            var section = data.Subdeeds.Where(x => x.SubUICTypeCode == sectionCode).FirstOrDefault();
            if (section != null)
            {
                return section.Records.Where(x => x.RecordCode == recordCode).Select(x => x.Value).FirstOrDefault();
            }
            return "";
        }

        private static string getDeedRecordFromArray(this IO.RegixClient.Models.RegixData.RegixActualStateV3ResponseVM data, string sectionCode, string recordCode)
        {
            var section = data.Subdeeds.Where(x => x.SubUICTypeCode == sectionCode).FirstOrDefault();
            if (section != null)
            {
                string result = "";
                foreach (var item in section.Records.Where(x => x.RecordCode == recordCode).Select(x => x.Value))
                {
                    result += item + ";";
                }
                return result;
            }
            return "";
        }

        private static decimal? ToDecimalSafe(this string model, decimal? emptyValue = null)
        {
            if (string.IsNullOrEmpty(model))
            {
                return emptyValue;
            }
            try
            {
                return decimal.Parse(model);
            }
            catch
            {
                return emptyValue;
            }

        }

        #endregion

        #region Регистър БУЛСТАТ

        public static void ToOrganization(this IO.RegixClient.Models.RegixData.RegixStateOfPlayResponseVM data, EntityInfoVM organization)
        {

            if (data.StateSubject.StateLegalEntitySubject != null)
            {
                organization.Label = data.StateSubject.StateLegalEntitySubject.CyrillicFullName;
                organization.LegalForm = data.StateSubject.StateLegalEntitySubject.LegalForm;
            }

            if (data.StateSubject.StateNaturalPersonSubject != null)
            {
                organization.Label = data.StateSubject.StateNaturalPersonSubject.CyrillicName;
                organization.LegalForm = "Физическо лице";
            }

            organization.HeadquatersAddress = data.StateSubject.StateAddress.Where(x => x.AddressType.Contains("управ")).Select(x => x.AddressText).FirstOrDefault();
            if (string.IsNullOrEmpty(organization.HeadquatersAddress))
                organization.HeadquatersAddress = data.StateSubject.StateAddress.Where(x => x.AddressType.Contains("местоиз")).Select(x => x.AddressText).FirstOrDefault();
            organization.LegalOrganManagers = data.getBSmanagers();
            organization.MainActivity = data.MainActivity2008;
            if (string.IsNullOrEmpty(organization.MainActivity))
            {
                organization.MainActivity = data.MainActivity2003;
            }
        }

        private static string getBSmanagers(this IO.RegixClient.Models.RegixData.RegixStateOfPlayResponseVM model)
        {
            string result = "";
            foreach (var item in model.StateManager)
            {
                result += $"{item.getBSmanagerName()}, {item.getBSmanagerUIC()};";
            }
            foreach (var item in model.CollectiveBodies)
            {
                foreach (var cb in item.StateMembers)
                {
                    result += $"{cb.getBSmanagerName()}, {cb.getBSmanagerUIC()};";
                }
            }
            return result.TrimEnd(';');
        }


        private static string getBSmanagerName(this IO.RegixClient.Models.RegixData.RegixStateManagerVM model)
        {
            string result = "";
            if (model.RelatedSubject.StateLegalEntitySubject != null)
            {
                result = model.RelatedSubject.StateLegalEntitySubject.CyrillicFullName;
            }
            if (model.RelatedSubject.StateNaturalPersonSubject != null)
            {
                result = model.RelatedSubject.StateNaturalPersonSubject.CyrillicName;
            }
            result += $", {model.Position}";
            return result;
        }

        private static string getBSmanagerUIC(this IO.RegixClient.Models.RegixData.RegixStateManagerVM model)
        {
            string result = "";
            if (model.RelatedSubject.StateLegalEntitySubject != null)
            {
                result = model.RelatedSubject.StateLegalEntitySubject.CyrillicFullName;
            }
            if (model.RelatedSubject.StateNaturalPersonSubject != null)
            {
                if (!string.IsNullOrEmpty(model.RelatedSubject.StateNaturalPersonSubject.EGN))
                {
                    result = $"ЕГН {model.RelatedSubject.StateNaturalPersonSubject.EGN}";
                }
                if (!string.IsNullOrEmpty(model.RelatedSubject.StateNaturalPersonSubject.LNC))
                {
                    result = $"ЛНЧ {model.RelatedSubject.StateNaturalPersonSubject.LNC}";
                }
            }
            result += $", {model.Position}";
            return result;
        }

        #endregion

    }
}

using eCase.Common.Crypto;
using eCase.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web.Models.Case
{
    public class CaseSearchVM
    {
        public static string MessageCaptcha = "Невалиден код за сигурност.";

        #region Search parameters

        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "{0} може да съдържа само цифри.")]
        [MaxLength(9, ErrorMessage = "{0} може да съдържа най-много 9 цифри.")]
        [Display(Name = "Входящ номер")]
        public string IncomingNumber { get; set; }

        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "{0} може да съдържа само цифри.")]
        [MaxLength(9, ErrorMessage = "{0} може да съдържа най-много 9 цифри.")]
        [Display(Name = "Дело номер")]
        public string Number { get; set; }

        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "{0} може да съдържа само цифри.")]
        [MaxLength(9, ErrorMessage = "{0} може да съдържа най-много 9 цифри.")]
        [Display(Name = "Дело година")]
        public string Year { get; set; }

        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "{0} може да съдържа само цифри.")]
        [MaxLength(9, ErrorMessage = "{0} може да съдържа най-много 9 цифри.")]
        [Display(Name = "Предходно дело номер")]
        public string PredecessorNumber { get; set; }

        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "{0} може да съдържа само цифри.")]
        [MaxLength(9, ErrorMessage = "{0} може да съдържа най-много 9 цифри.")]
        [Display(Name = "Преходно дело година")]
        public string PredecessorYear { get; set; }

        public string CaseKindId { get; set; }
        public string SideName { get; set; }
        public string CourtCode { get; set; }
        public string LawyerId { get; set; }
        public bool AreOnlyPersonalCases { get; set; }

        public string ActKindId { get; set; }
        public string ActNumber { get; set; }
        public string ActYear { get; set; }

        public bool HasPersonalCases { get; set; }
        public bool ShowLawyers { get; set; }
        public bool ShowSides { get; set; }

        public bool ShowResults { get; set; }

        public CasesOrder Order { get; set; }
        public bool IsAsc { get; set; }

        // public bool RestrictedAccess { get; set; } //comment for PROD
        #endregion

        #region Selects

        public IEnumerable<SelectListItem> Courts { get; set; }
        public IEnumerable<SelectListItem> CaseKinds { get; set; }
        public IEnumerable<SelectListItem> ActKinds { get; set; }

        public IEnumerable<SelectListItem> CaseYears { get; set; }
        public IEnumerable<SelectListItem> PredecessorYears { get; set; }
        public IEnumerable<SelectListItem> ActYears { get; set; }

        #endregion

        #region SearchResults

        public PagedList.IPagedList<eCase.Domain.Entities.Case> SearchResults { get; set; }

        #endregion

        #region Statics

        public static void EncryptProperties(CaseSearchVM vm)
        {
            vm.IncomingNumber = ConfigurationBasedStringEncrypter.Encrypt(vm.IncomingNumber);
            vm.Number = ConfigurationBasedStringEncrypter.Encrypt(vm.Number);
            vm.Year = ConfigurationBasedStringEncrypter.Encrypt(vm.Year);
            vm.PredecessorNumber = ConfigurationBasedStringEncrypter.Encrypt(vm.PredecessorNumber);
            vm.PredecessorYear = ConfigurationBasedStringEncrypter.Encrypt(vm.PredecessorYear);
            vm.CaseKindId = ConfigurationBasedStringEncrypter.Encrypt(vm.CaseKindId);
            vm.SideName = ConfigurationBasedStringEncrypter.Encrypt(vm.SideName);
            vm.CourtCode = ConfigurationBasedStringEncrypter.Encrypt(vm.CourtCode);
            vm.LawyerId = ConfigurationBasedStringEncrypter.Encrypt(vm.LawyerId);
            vm.ActKindId = ConfigurationBasedStringEncrypter.Encrypt(vm.ActKindId);
            vm.ActNumber = ConfigurationBasedStringEncrypter.Encrypt(vm.ActNumber);
            vm.ActYear = ConfigurationBasedStringEncrypter.Encrypt(vm.ActYear);
            vm.ActYear = ConfigurationBasedStringEncrypter.Encrypt(vm.ActYear);
        }

        #endregion

        #region RouteValues

        public object GetRouteValues(NameValueCollection queryString
            , string page = ""
            , CasesOrder? order = null)
        {
            string isAscString = queryString["isAsc"];

            if (order.HasValue)
            {
                if (this.Order == order.Value)
                    isAscString = (!this.IsAsc).ToString();
                else
                    isAscString = true.ToString();
            }

            var result = new
            {
                incomingNumber = queryString["incomingNumber"],
                number = queryString["number"],
                year = queryString["year"],
                predecessorNumber = queryString["predecessorNumber"],
                predecessorYear = queryString["predecessorYear"],
                caseKindId = queryString["caseKindId"],
                sideName = queryString["sideName"],
                courtCode = queryString["courtCode"],
                lawyerId = queryString["lawyerId"],
                actKindId = queryString["actKindId"],
                actNumber = queryString["actNumber"],
                actYear = queryString["actYear"],

                areOnlyPersonalCases = queryString["areOnlyPersonalCases"],
                showResults = true,

                page = page,

                order = order.HasValue ? order.ToString() : queryString["order"],
                isAsc = isAscString
            };

            return result;
        }

        #endregion
    }
}
using eCase.Common.Crypto;
using eCase.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eCase.Web.Models.Summon
{
    public class SummonSearchVM
    {
        #region Search parameters

        public string CourtCode { get; set; }
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\.(0[13578]|1[02])\.((1[6-9]|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\.(0[13456789]|1[012])\.((1[6-9]|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\.02\.((1[6-9]|[2-9]\d)\d{2}))|(29\.02\.((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Дата от трябва да бъде във формат дд.мм.гггг.")]
        public string DateFrom { get; set; }
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\.(0[13578]|1[02])\.((1[6-9]|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\.(0[13456789]|1[012])\.((1[6-9]|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\.02\.((1[6-9]|[2-9]\d)\d{2}))|(29\.02\.((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Дата до трябва да бъде във формат дд.мм.гггг.")]
        public string DateTo { get; set; }

        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "{0} може да съдържа само цифри.")]
        [MaxLength(9, ErrorMessage = "{0} може да съдържа най-много 9 цифри.")]
        [Display(Name = "Дело номер")]
        public string CaseNumber { get; set; }

        [RegularExpression(@"^\d{0,20}$", ErrorMessage = "{0} може да съдържа само цифри.")]
        [MaxLength(9, ErrorMessage = "{0} може да съдържа най-много 9 цифри.")]
        [Display(Name = "Дело година")]
        public string CaseYear { get; set; }
        public bool IsOnlyUnread { get; set; }

        public string Type { get; set; }

        public SummonsOrder Order { get; set; }
        public bool IsAsc { get; set; }

        #endregion

        #region Selects

        public IEnumerable<SelectListItem> Courts { get; set; }
        public IEnumerable<SelectListItem> CaseYears { get; set; }
        public IEnumerable<SelectListItem> Types { get; set; }

        #endregion

        #region SearchResults

        public PagedList.IPagedList<eCase.Domain.Entities.Summon> SearchResults { get; set; }

        #endregion

        #region Statics

        public static void EncryptProperties(SummonSearchVM vm)
        {
            vm.CourtCode = ConfigurationBasedStringEncrypter.Encrypt(vm.CourtCode);
            vm.DateFrom = ConfigurationBasedStringEncrypter.Encrypt(vm.DateFrom);
            vm.DateTo = ConfigurationBasedStringEncrypter.Encrypt(vm.DateTo);
            vm.CaseNumber = ConfigurationBasedStringEncrypter.Encrypt(vm.CaseNumber);
            vm.CaseYear = ConfigurationBasedStringEncrypter.Encrypt(vm.CaseYear);
            vm.Type = ConfigurationBasedStringEncrypter.Encrypt(vm.Type);
        }

        #endregion

        #region RouteValues

        public object GetRouteValues(NameValueCollection queryString
            , string page = ""
            , SummonsOrder? order = null)
        {
            string isAscString = queryString["isAsc"];

            if(order.HasValue)
            {
                if (this.Order == order.Value)
                    isAscString = (!this.IsAsc).ToString();
                else
                    isAscString = true.ToString();
            }
                
            var result = new
            {
                courtId = queryString["courtCode"],
                dateFrom = queryString["dateFrom"],
                dateTo = queryString["dateTo"],
                caseNumber = queryString["caseNumber"],
                caseYear = queryString["caseYear"],
                type = queryString["type"],
                isOnlyUnread = queryString["isOnlyUnread"],

                page = page,

                order = order.HasValue ? order.ToString() : queryString["order"],
                isAsc = isAscString
            };

            return result;
        }

        #endregion
    }
}
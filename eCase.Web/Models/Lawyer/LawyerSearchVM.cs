using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using PagedList;

using eCase.Common.Crypto;

namespace eCase.Web.Models.Lawyer
{
    public class LawyerSearchVM
    {
        #region Search parameters

        [Display(Name = "Номер")]
        public string Number { get; set; }

        [Display(Name = "Име")]
        public string Name { get; set; }

        public string LawyerTypeId { get; set; }

        #endregion

        #region Selects

        public IEnumerable<SelectListItem> LawyerTypes { get; set; }

        #endregion

        #region SearchResults

        public IPagedList<Domain.Entities.Lawyer> SearchResults { get; set; }

        #endregion

        #region Statics

        public static void EncryptProperties(LawyerSearchVM vm)
        {
            vm.Number = ConfigurationBasedStringEncrypter.Encrypt(vm.Number);
            vm.Name = ConfigurationBasedStringEncrypter.Encrypt(vm.Name);
            vm.LawyerTypeId = ConfigurationBasedStringEncrypter.Encrypt(vm.LawyerTypeId);
        }

        #endregion
    }
}
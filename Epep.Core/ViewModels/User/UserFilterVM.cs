using Epep.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.User
{
    public class UserFilterVM
    {
        [Display(Name = "Вид потребител")]
        public int? UserType { get; set; }
        [Display(Name = "Имена/наименование")]
        public string FullName { get; set; }
        [Display(Name = "ЕГН/ЕИК")]
        public string UIC { get; set; }
        [Display(Name = "Електронна поща")]
        public string Email { get; set; }

        [Display(Name = "Потвърдени/Непотвърдени ЮЛ")]
        public int? ComfirmedMode { get; set; }

        [Display(Name = "Само активни потребители")]
        public bool ActiveOnly { get; set; }

        public UserFilterVM()
        {
            ActiveOnly = true;
        }

        public void Sanitize()
        {
            if (UserType < 0)
            {
                UserType = null;
            }
            if (ComfirmedMode < 0)
            {
                ComfirmedMode = null;
            }

            FullName = FullName.EmptyToNull();
            UIC = UIC.EmptyToNull();
            Email = Email.EmptyToNull();
        }
    }
}

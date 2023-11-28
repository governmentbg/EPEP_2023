using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Regix;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.User
{
    public class UserComfirmVM
    {
        public Guid Gid { get; set; }

        [Display(Name = "Вид")]
        public int UserType { get; set; }
        public string UserTypeName { get; set; }
        [Display(Name = "Имена")]
        public string FullName { get; set; }
        [Display(Name = "Организация")]
        public string OrganizationName { get; set; }
        [Display(Name = "Идентификатор")]
        public string OrganizationUic { get; set; }
      
        [Display(Name = "Идентификатор")]
        public string Uic { get; set; }
        [Display(Name = "Електронна поща")]
        public string Email { get; set; }

        [Display(Name = "Данни от КЕП")]
        public string RegCertificateInfo { get; set; }

        [Display(Name = "Потвърждение")]
        public bool Comfirm { get; set; }

        [Display(Name = "Основание")]
        public string DeniedDescription { get; set; }

        public List<FileItemVM> Files { get; set; }

    }
}

using Epep.Core.ViewModels.Case;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.User
{
    public class UserComfirmedInfoVM
    {
        public long Id { get; set; }
        [Display(Name = "Потвърдена регистрация на")]
        public DateTime? ComfirmedDate { get; set; }
        public string ComfirmedUserName { get; set; }
        public string ComfirmedUserType { get; set; }
        [Display(Name = "Отказана регистрация на")]
        public DateTime? DeniedDate { get; set; }
        [Display(Name = "Основание")]
        public string DeniedDescription { get; set; }

        public List<FileItemVM> Files { get; set; }
    }
}

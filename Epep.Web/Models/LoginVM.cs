using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Epep.Web.Models
{
    public class LoginVM
    {
        [Display(Name = "Вид лице")]
        public int LoginUserType { get; set; }

        [Display(Name = "Идентификатор")]
        public string UIC { get; set; }

        [Display(Name = "ЕИК Организация")]
        public string OrganizationUIC { get; set; }

        public string ExternalProvider { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }
    }
}

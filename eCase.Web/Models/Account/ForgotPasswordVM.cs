using System.ComponentModel.DataAnnotations;

namespace eCase.Web.Models.Account
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage = "Полето \"Потребителско име\", съдържащо Вашия имейл адрес, е задължително.")]
        [StringLength(100, ErrorMessage = "Полето \"Потребителско име\" не може да съдържа повече от 100 символа.")]
        [RegularExpression(@"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?", ErrorMessage = "Полето \"Потребителско име\", съдържащо Вашия имейл адрес, е невалидно.")]
        public string Email { get; set; }
    }
}
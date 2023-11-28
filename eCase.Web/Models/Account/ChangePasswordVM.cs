using System.ComponentModel.DataAnnotations;

using eCase.Web.Helpers;

namespace eCase.Web.Models.Account
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Полето \"Стара парола\" е задължително.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Полето \"Нова парола\" е задължително.")]
        [ValidPassword(100, MinimumLength = 8, ErrorMessage = "Паролата трябва да съдържа минимум осем знака, като поне два от тях трябва да са: главни букви, малки букви, числа или специални символи. Пример: Z@sht1tn1k.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Полетата \"Нова парола\" и \"Въведи новата парола повторно\" не съвпадат.")]
        [Required(ErrorMessage = "Полето \"Въведи новата парола повторно\" е задължително.")]
        public string ConfirmPassword { get; set; } 
    }
}
using System;
using System.ComponentModel.DataAnnotations;

using eCase.Web.Helpers;

namespace eCase.Web.Models.Account
{
    public class ActivateVM
    {
        public Guid UserGid { get; set; }

        [Required(ErrorMessage = "Полето \"Парола\" е задължително.")]
        [ValidPassword(100, MinimumLength = 8, ErrorMessage = "Паролата трябва да съдържа минимум осем знака, като поне два от тях трябва да са: главни букви, малки букви, числа или специални символи. Пример: Z@sht1tn1k.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Полетата \"Парола\" и \"Въведи паролата повторно\" не съвпадат.")]
        [Required(ErrorMessage = "Полето \"Въведи паролата повторно\" е задължително.")]
        public string ConfirmPassword { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\.(0[13578]|1[02])\.((1[6-9]|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\.(0[13456789]|1[012])\.((1[6-9]|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\.02\.((1[6-9]|[2-9]\d)\d{2}))|(29\.02\.((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Дата до трябва да бъде във формат дд.мм.гггг.")]
        [Required(ErrorMessage = "Полето \"Рождена дата\" е задължително.")]
        public string BirthDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VSS.Portal.Models.Feedback
{
    public class FeedbackVM
    {
        [Required(ErrorMessage = "Изборът на поле „Съобщение за“ е задължително.")]
        public string Type { get; set; }

        [StringLength(100, ErrorMessage = "Име и фамилия не могат да съдържат повече от 100 символа.")]
        [RegularExpression(@"^[а-яА-Яa-zA-Z ]*$", ErrorMessage = "Име и фамилия трябва да съдържат само букви.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Електронната поща не трябва да надвишава 100 символа.")]
        [RegularExpression(@"^[\w\-!#$%&'*+/=?^`{|}~.""]+@([\w]+[.-]?)+[\w]\.[\w]+$", ErrorMessage = "Невалидна електронна поща.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Текстът в полето „Описание“ е задължителен.")]
        [StringLength(4000, ErrorMessage = "Текстът в полето „Описание“ не може да съдържа повече от 4000 символа.")]
        public string Message { get; set; }

        public string Captcha { get; set; }
    }
}
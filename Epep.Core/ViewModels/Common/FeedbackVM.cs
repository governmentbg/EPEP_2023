using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Common
{
    public class FeedbackVM
    {
        [Range(1, 4, ErrorMessage = "Изборът на поле „Съобщение за“ е задължително.")]
        [Display(Name = "СЪобщение за")]
        public int Type { get; set; }
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case 1:
                        return "Въпрос";
                    case 2:
                        return "Предложение";
                    case 3:
                        return "Технически проблем";
                    default:
                        return string.Empty;
                }
            }
        }

        [Required(ErrorMessage = "Полето „{0}“ е задължително.")]
        [StringLength(100, ErrorMessage = "Полето „{0}“ не можe да съдържа повече от 100 символа.")]
        [RegularExpression(@"^[а-яА-Яa-zA-Z ]*$", ErrorMessage = "Полето „{0}“ трябва да съдържа само букви.")]
        [Display(Name = "Три имена")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето „{0}“ е задължително.")]
        [StringLength(100, ErrorMessage = "Електронната поща не трябва да надвишава 100 символа.")]
        [RegularExpression(@"^[\w\-!#$%&'*+/=?^`{|}~.""]+@([\w]+[.-]?)+[\w]\.[\w]+$", ErrorMessage = "Невалидна електронна поща.")]
        [Display(Name = "Електронна поща")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Полето „{0}“ е задължително.")]
        [StringLength(4000, ErrorMessage = "Текстът в полето „Описание“ не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Описание")]
        public string Message { get; set; }

        public string reCaptchaToken { get; set; }
    }
}
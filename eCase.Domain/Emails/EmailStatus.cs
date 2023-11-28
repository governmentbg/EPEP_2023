using System.ComponentModel;

namespace eCase.Domain.Emails
{
    public enum EmailStatus
    {
        [Description("Предстоящо изпращане")]
        Pending = 1,

        [Description("Изпратен")]
        Sent = 2,

        [Description("Грешка")]
        UknownError = 3,
    }
}
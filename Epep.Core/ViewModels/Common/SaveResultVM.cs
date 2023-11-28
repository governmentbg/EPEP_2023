using Newtonsoft.Json;

namespace Epep.Core.ViewModels.Common
{
    public class SaveResultVM
    {
        public const string MessageSaveOk = "Записът премина успешно.";
        public const string MessageSaveFailed = "Проблем по време на запис.";
        public const string MessageNotFound = "Търсения от вас обект не е намерен.";
        public const string MessageAccessDenied = "Нямате достъп до търсения обект.";

        private bool result;
        public bool Result
        {
            get
            {
                if (Errors.Count > 0)
                {
                    return false;
                }
                return result;
            }
            set
            {
                result = value;
            }
        }
        public string Message { get; set; }

        [JsonIgnore]
        public string AuditInfo { get; set; }

        public List<SaveResultError> Errors { get; set; }

        public SaveResultVM()
        {
            Errors = new List<SaveResultError>();
            Result = true;
        }
        public SaveResultVM(bool result, string message = null)
        {
            Result = result;
            Errors = new List<SaveResultError>();
            Message = message;
            if (!result && string.IsNullOrEmpty(message))
            {
                Message = MessageSaveFailed;
            }
            if (result && string.IsNullOrEmpty(message))
            {
                Message = MessageSaveOk;
            }


        }
        public void AddError(string error, string control = null)
        {
            Errors.Add(new SaveResultError()
            {
                Control = control,
                Error = error
            });
        }

        public object ParentId { get; set; }
        public object ObjectId { get; set; }
    }

    public class SaveResultError
    {
        public string Control { get; set; }
        public string Error { get; set; }

    }
}

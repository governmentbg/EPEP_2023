namespace Epep.Core.ViewModels.User
{
    public class UserVacationVM
    {
        public long Id { get; set; }
        public string VacationType { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string DocumentNumber { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Report
{
    public class ReportLawyerViewVM
    {
        [Display(Name = "Съд")]
        public string CourtName { get; set; }
        public string CaseKind { get; set; }
        [Display(Name = "Вид дело")]
        public string CaseKindName { get; set; }
        [Display(Name = "Номер дело")]
        public int CaseNumber { get; set; }
        [Display(Name = "Година")]
        public int CaseYear { get; set; }

        public Guid CaseGid { get; set; }
        [Display(Name = "Дата на достъпа")]
        public DateTime? ViewDate { get; set; }
    }
}

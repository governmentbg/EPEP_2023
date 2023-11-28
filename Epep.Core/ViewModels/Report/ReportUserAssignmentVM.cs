using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Report
{
    public class ReportUserAssignmentVM
    {
        [Display(Name ="Съд")]
        public string CourtName { get; set; }
        public string CaseKind { get; set; }
        [Display(Name ="Вид дело")]
        public string CaseKindName { get; set; }
        [Display(Name ="Номер дело")]
        public int CaseNumber { get; set; }
        [Display(Name ="Година")]
        public int CaseYear { get; set; }

        [Display(Name ="Страна")]
        public string SideName { get; set; }
        [Display(Name ="Качество")]
        public string SideRoleName { get; set; }
        [Display(Name ="Достъп в качество на")]
        public string AssignmentRole { get; set; }
        [Display(Name ="Дата на добавяне")]
        public DateTime CreateDate { get; set; }
    }
}

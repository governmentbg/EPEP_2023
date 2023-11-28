using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Report
{
    public class ReportCaseStatsVM
    {
        [Display(Name="Съд")]
        public string CourtName { get; set; }
        [Display(Name="Граждански")]
        public int Grd { get; set; }
        [Display(Name="Наказателни")]
        public int Nkz { get; set; }
        [Display(Name="Фирмени")]
        public int Frm { get; set; }
        [Display(Name="Търговски")]
        public int Trg { get; set; }
        [Display(Name="Административни")]
        public int Adm { get; set; }
        [Display(Name="ОБЩО")]
        public int Total
        {
            get
            {
                return Grd + Nkz + Frm + Trg + Adm;
            }
        }
    }
}

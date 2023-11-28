namespace IOWebApplication.Infrastructure.Models.ViewModels.Common
{
    public class CalendarVM
    {
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
        public long? id { get; set; }
        public bool allDay { get; set; }
        public string[] classNames { get; set; }
    }

    public class CalendarDateVM
    {
        public bool IsToday { get { return Date.Date == DateTime.Now.Date; } }
        public DateTime Date { get; set; }
        public CalendarDateEventVM[] Events { get; set; }
    }

    public class CalendarDateEventVM
    {
        public Guid Gid { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string CourtName { get; set; }
        public Guid? CaseGid { get; set; }
        public string CaseInfo { get; set; }
    }
}

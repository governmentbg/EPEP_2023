namespace Epep.Core.ViewModels.Case
{
    public class HearingVM
    {
        public Guid Gid { get; set; }
        public DateTime Date { get; set; }
        public string HearingType { get; set; }
        public string HearingResult { get; set; }
        public bool IsCanceled { get; set; }
        public string SecretaryName { get; set; }
        public string ProsecutorName { get; set; }
        public string CourtName { get; set; }
        public Guid CaseGid { get; set; }
        public string CaseInfo { get; set; }
        public string CourtRoom { get; set; }
        public string VideoUrl { get; set; }
    }

    public class HearingOnlineVM
    {
        public Guid Gid { get; set; }
        public Guid CaseGid { get; set; }
        public DateTime Date { get; set; }
        public string CourtName { get; set; }
        public string CaseKindName { get; set; }
        public int CaseNumber { get; set; }
        public int CaseYear { get; set; }
        public string HearingType { get; set; }
        public string HearingResult { get; set; }
        public bool IsCanceled { get; set; }
        public string VideoUrl { get; set; }
    }
}

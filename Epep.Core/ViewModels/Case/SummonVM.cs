namespace Epep.Core.ViewModels.Case
{
    public class SummonVM
    {
        public Guid Gid { get; set; }
        public string SummonType { get; set; }
        public string SummonKind { get; set; }
        public string Number { get; set; }
        public string CourtName { get; set; }
        public string CaseInfo { get; set; }
        public string CaseGid { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateServed { get; set; }
        public string Subject { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadTime { get; set; }

    }
}

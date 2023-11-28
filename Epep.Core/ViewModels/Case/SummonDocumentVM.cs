namespace Epep.Core.ViewModels.Case
{
    public class SummonDocumentVM
    {
        public string CourtName { get; set; }

        public int CaseNumber { get; set; }

        public string CaseKind { get; set; }

        public int CaseYear { get; set; }

        public string SummonKind { get; set; }
        public string UserNameRead { get; set; }
        public string UserEmailRead { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime ReadTime { get; set; }

        public Guid? SummonBlobKey { get; set; }
        public byte[] SummonContent { get; set; }

        public string Addressee { get; set; }
    }
}
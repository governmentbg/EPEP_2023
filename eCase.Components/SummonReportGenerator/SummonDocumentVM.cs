using System;

namespace eCase.Components.SummonReportGenerator
{
    public class SummonDocumentVM
    {
        public string CourtName { get; set; }

        public int CaseNumber { get; set; }

        public string CaseKind { get; set; }

        public int CaseYear { get; set; }

        public string SummonKind { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime ReadTime { get; set; }

        public string Addressee { get; set; }
    }
}
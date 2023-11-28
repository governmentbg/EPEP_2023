using Epep.Core.Constants;

namespace Epep.Core.ViewModels.Case
{
    public class CaseElementVM
    {
        public int OrderBy { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public Guid Gid { get; set; }
        public string ItemType { get; set; }
        public string Number { get; set; }
        public string DetailTitle { get; set; }
        public string Detail { get; set; }
        public DateTime Date { get; set; }

        public string TypeIcon
        {
            get
            {
                switch (Type)
                {
                    case NomenclatureConstants.SourceTypes.IncommingDocument:
                        return "timeline__icon--doc-in";
                    case NomenclatureConstants.SourceTypes.OutgoingDocument:
                        return "timeline__icon--doc-out";
                    case NomenclatureConstants.SourceTypes.Hearing:
                        return "timeline__icon--zasedanie";
                    case NomenclatureConstants.SourceTypes.Act:
                        return "timeline__icon--act";
                    case NomenclatureConstants.SourceTypes.Assignment:
                        return "timeline__icon--razpredelenie";
                    case NomenclatureConstants.SourceTypes.Side:
                        return "timeline__icon--side";                   
                    default:
                        return string.Empty;
                }
            }
        }
    }
}

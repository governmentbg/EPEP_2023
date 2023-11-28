
namespace Epep.Core.ViewModels.Case
{
    public class FilesPreviewVM
    {
        public long ObjectId { get; set; }
        public long CourtId { get; set; }
        public long CaseId { get; set; }
        public Guid ObjectGid { get; set; }
        public string BackUrl { get; set; }
        public string BackCanvasUrl { get; set; }
        public int Type { get; set; }
        public string Number { get; set; }
        public string TypeName { get; set; }
        public string TypeDetails { get; set; }
        public DateTime? Date { get; set; }
        public bool ForAction { get; set; }
        public string ActionMode { get; set; }
        public string SummonCourtDescription { get; set; }
        public DateTime? SummonCourtDate { get; set; }
        public List<FileItemVM> Files { get; set; }
        public List<ActAppealVM> ActAppeals { get; set; }

    }
    public class FileItemVM
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public Guid FileGid { get; set; }

        public bool CanPreview
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FileName))
                {
                    return false;
                }
                return FileName.ToLower().EndsWith("pdf");
            }
        }
    }

    public class FileItemApiVM : FileItemVM
    {
        public string FileUrl { get; set; }
    }
}

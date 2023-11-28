namespace Epep.Core.ViewModels.Common
{
    public class FileListVM
    {
        public string FileName { get; set; }
        public string Hash { get; set; }
        public Guid Gid { get; set; }
        public Guid BlobKey { get; set; }
        public string SignersInfo { get; set; }
    }
}

namespace Epep.Core.ViewModels.Common
{
    public class BlobInfo
    {
        public Guid BlobKey { get; set; }
        public long BlobContentId { get; set; }
        public long BlobContentLocationId { get; set; }
        public string ContentDbConnectionStringName { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
    }

    public class RangeInfo
    {
        public long From { get; set; }
        public long To { get; set; }
        public long Length { get; set; }
    }

    public class BlobUploadResultVM
    {
        public bool Result { get; set; }
        public long BlobContentId { get; set; }
        public string StorageName { get; set; }
    }
}

using Epep.Core.Services;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;

namespace Epep.Core.Contracts
{
    public interface IBlobService
    {
        Task<bool> HasKey(Guid blobKey);
        Task<BlobInfo> GetBlobInfo(Guid key, CancellationToken cancellationToken = default);
        string GetMimeType(string fileName);
        Task CopyBlobContentToResponseStream(Stream responseStream, RangeInfo rangeInfo, BlobInfo blobInfo, CancellationToken cancellationToken);
        Task<byte[]> GetFileContent(Guid blobKey, CancellationToken cancellationToken = default);
        Task<Guid> UploadFileToBlobStorage(Guid blobKey, byte[] content, string mimeType, BlobServiceBase.FileType fileType, DateTime fileDate, bool isUpdate = false, string originalFileName = null, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileFromStorage(Guid blobKey, CancellationToken cancellationToken = default);
        IQueryable<FileItemVM> SelectAttachedDocument(int attachmentType, long parentId);
        Task<BlobInfo> AppendUpdateAttachedDocumentFile(int attachedType, long parentId, byte[] fileBytes, string fileName, bool singleFileOnly = true, string fileTitle = null, CancellationToken cancellationToken = default);
    }
}

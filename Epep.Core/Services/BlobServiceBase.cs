using Epep.Core.Constants;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Blob = Epep.Core.Models.Blob;

namespace Epep.Core.Services
{
    public class BlobServiceBase
    {
        protected IConfiguration config;
        protected IRepository repo;
        protected virtual async Task<BlobUploadResultVM> UploadFile(string hash, byte[] content, CancellationToken cancellationToken) { return await Task.FromResult(new BlobUploadResultVM() { Result = false }); }
        protected virtual async Task<bool> RemoveBlobContent(BlobInfo blobInfo, CancellationToken cancellationToken) { return await Task.FromResult(false); }
        protected virtual async Task<bool> ExistsBlobContentId(BlobInfo blobInfo, CancellationToken cancellationToken) { return await Task.FromResult(false); }

        public async Task<BlobInfo> GetBlobInfo(Guid key, CancellationToken cancellationToken = default)
        {
            return await repo.AllReadonly<Blob>()
                            .Where(x => x.Key == key)
                            .Select(x => new BlobInfo
                            {
                                BlobKey = x.Key,
                                BlobContentId = x.BlobContentLocation.BlobContentId,
                                BlobContentLocationId = x.BlobContentLocationId,
                                ContentDbConnectionStringName = x.BlobContentLocation.ContentDbConnectionStringName,
                                FileLength = x.BlobContentLocation.Size,
                                FileName = x.FileName
                            }).FirstOrDefaultAsync(cancellationToken);
        }

        public string GetMimeType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            var mimeType = MimeTypeHelper.GetFileMimeTypeByExtenstion(fileExtension.ToLower());

            return mimeType;
        }

        protected string ComputeHash(byte[] content)
        {
            var result = string.Empty;
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(content);
                result = hash.ToHexString(false);
            }

            return result;
        }

        public async Task<Guid> UploadFileToBlobStorage(Guid blobKey, byte[] content, string mimeType, FileType fileType, DateTime fileDate, bool isUpdate = false, string originalFileName = null, CancellationToken cancellationToken = default)
        {
            if (content == null)
            {
                return Guid.Empty;
            }
            var hash = ComputeHash(content);

            var uploadResult = await UploadFile(hash, content, cancellationToken);
            if (!uploadResult.Result)
            {
                return Guid.Empty;
            }

            var fileName = GenerateFileName(fileType, fileDate, mimeType);
            if (FileTypesPreserveFileName.Contains(fileType) && !string.IsNullOrEmpty(originalFileName))
            {
                fileName = originalFileName;
            }
            if (isUpdate)
            {
                var blob = await repo.GetByIdAsync<Blob>(blobKey);
                blob.FileName = fileName;
                var blobLocation = await repo.GetByIdAsync<BlobContentLocation>(blob.BlobContentLocationId);
                blobLocation.BlobContentId = uploadResult.BlobContentId;
                blobLocation.ContentDbConnectionStringName = uploadResult.StorageName;
                blobLocation.Hash = hash;
                blobLocation.Size = content.Length;
            }
            else
            {
                var blob = new Blob()
                {
                    BlobContentLocation = new BlobContentLocation()
                    {
                        BlobContentId = uploadResult.BlobContentId,
                        ContentDbConnectionStringName = uploadResult.StorageName,
                        Hash = hash,
                        Size = content.Length
                    },
                    Key = blobKey,
                    FileName = fileName
                };
                await repo.AddAsync(blob);
            }
            await repo.SaveChangesAsync();
            return blobKey;
        }

        public async Task<bool> DeleteFileFromStorage(Guid blobKey, CancellationToken cancellationToken = default)
        {
            var blobInfo = await GetBlobInfo(blobKey, cancellationToken);
            if (blobInfo == null)
            {
                return false;
            }

            var blobDeleteResult = await RemoveBlobContent(blobInfo, cancellationToken);
            if (!blobDeleteResult)
            {
                blobDeleteResult = !(await ExistsBlobContentId(blobInfo, cancellationToken));
            }
            if (blobDeleteResult)
            {
                var blobLocation = await repo.GetByIdAsync<BlobContentLocation>(blobInfo.BlobContentLocationId);
                if(blobLocation != null)
                {
                    blobLocation.ContentDbConnectionStringName = "deleted";
                    blobLocation.Hash = $"{DateTime.Now:dd.MM.yyyy hh:mm:ss}";
                }
                //await repo.DeleteAsync<BlobContentLocation>(blobInfo.BlobContentLocationId);
                //await repo.DeleteAsync<Blob>(blobKey);
                return true;
            }
            return false;
        }

        public IQueryable<FileItemVM> SelectAttachedDocument(int attachmentType, long parentId)
        {
            return repo.AllReadonly<AttachedDocument>()
                                    .Where(x => x.AttachmentType == attachmentType && x.ParentId == parentId)
                                    .OrderByDescending(x => x.AttachedDocumentId)
                                    .Select(x => new FileItemVM
                                    {
                                        Type = attachmentType,
                                        Title = x.FileTitle,
                                        FileGid = x.BlobKey,
                                        FileName = x.AttachedBlob.FileName
                                    });
        }

        public async Task<BlobInfo> AppendUpdateAttachedDocumentFile(int attachedType, long parentId, byte[] fileBytes, string fileName, bool singleFileOnly = true, string fileTitle = null, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }
            if (singleFileOnly)
            {
                var savedFiles = await repo.All<AttachedDocument>()
                                        .Where(x => x.AttachmentType == attachedType)
                                        .Where(x => x.ParentId == parentId)
                                        .ToListAsync();

                foreach (var savedFile in savedFiles)
                {
                    if (await DeleteFileFromStorage(savedFile.BlobKey))
                    {
                        repo.Delete(savedFile);
                        await repo.SaveChangesAsync();
                    }
                }
            }

            Guid blobKey = await UploadFileToBlobStorage(Guid.NewGuid(), fileBytes, this.GetMimeType(fileName), FileType.AttachedDocument, DateTime.Now, false, fileName, cancellationToken);

            if (blobKey == Guid.Empty)
            {
                return null;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var newDoc = new AttachedDocument()
            {
                AttachmentType = attachedType,
                ParentId = parentId,
                BlobKey = blobKey,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                FileName = fileName,
                FileTitle = fileTitle,
                Gid = Guid.NewGuid()
            };

            try
            {
                await repo.AddAsync(newDoc);
                await repo.SaveChangesAsync();
                return new BlobInfo()
                {
                    BlobKey = blobKey,
                    FileName = fileName
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> HasKey(Guid blobKey)
        {
            return (await repo.GetByIdAsync<Blob>(blobKey)) != null;
        }

        protected string GenerateFileName(FileType fileType, DateTime fileDate, string mimeType)
        {
            var suffix = string.Format("{0}_{1}_{2}{3}", fileDate.Year,
                  fileDate.Month, fileDate.Day, MimeTypeHelper.GetFileExtenstionByMimeType(mimeType));

            switch (fileType)
            {
                case FileType.Incoming:
                    return string.Format("incoming_{0}", suffix);
                case FileType.Outgoing:
                    return string.Format("outgoing_{0}", suffix);
                case FileType.PrivateAct:
                    return string.Format("act_{0}", suffix);
                case FileType.PublicAct:
                    return string.Format("act_{0}", suffix);
                case FileType.PrivateMotive:
                    return string.Format("motive_{0}", suffix);
                case FileType.PublicMotive:
                    return string.Format("motive_{0}", suffix);
                case FileType.PrivateProtocol:
                    return string.Format("protocol_{0}", suffix);
                case FileType.PublicProtocol:
                    return string.Format("protocol_{0}", suffix);
                case FileType.Assignment:
                    return string.Format("assignment_{0}", suffix);
                case FileType.Summon:
                    return string.Format("summon_{0}", suffix);
                case FileType.SummonTimeStamp:
                    return string.Format("summon_timestamp_{0}", suffix);
                case FileType.SummonReport:
                    return string.Format("summon_report_{0}", suffix);
                case FileType.ScannedDocument:
                    return string.Format("scanned_document_{0}", suffix);
                default:
                    return string.Format("document_{0}", suffix);
            }
        }
        public enum FileType
        {
            Incoming,
            Outgoing,
            PrivateAct,
            PublicAct,
            PrivateMotive,
            PublicMotive,
            PrivateProtocol,
            PublicProtocol,
            Assignment,
            Summon,
            SummonReport,
            SummonTimeStamp,
            ScannedDocument,
            AttachedDocument,
            ElectronicDocument,
            ElectronicDocumentStamp
        }

        public static FileType[] FileTypesPreserveFileName = { FileType.AttachedDocument, FileType.ElectronicDocumentStamp };
    }
}

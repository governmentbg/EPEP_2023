using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using IO.SignTools.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Epep.Core.Services
{
    public class SignToolsTempFileHelper : ITempFileHandler
    {
        private readonly IBlobService blobService;
        private readonly IRepository repo;
        public SignToolsTempFileHelper(
            IBlobService _blobService,
            IRepository _repo)
        {
            blobService = _blobService;
            repo = _repo;
        }

        public async Task DeleteFile(string filename)
        {
            var tmpFileDoc = await repo.AllReadonly<AttachedDocument>()
                                      .Where(x => x.AttachmentType == NomenclatureConstants.AttachedTypes.SignTempFile)
                                      .Where(x => x.ParentId == 0)
                                      .Where(x => x.FileName == filename)
                                      .FirstOrDefaultAsync();

            if (tmpFileDoc == null)
            {
                return;
            }

            if (await blobService.DeleteFileFromStorage(tmpFileDoc.BlobKey))
            {
                repo.Delete(tmpFileDoc);
                await repo.SaveChangesAsync();
            }
        }

        public async Task<byte[]> ReadFile(string filename)
        {
            var blobKey = await repo.AllReadonly<AttachedDocument>()
                                      .Where(x => x.AttachmentType == NomenclatureConstants.AttachedTypes.SignTempFile)
                                      .Where(x => x.ParentId == 0)
                                      .Where(x => x.FileName == filename)
                                      .Select(x => x.BlobKey)
                                      .FirstOrDefaultAsync();

            if (blobKey == Guid.Empty)
            {
                return null;
            }

            return await blobService.GetFileContent(blobKey);
        }

        public async Task SaveFile(string filename, byte[] data)
        {
            try
            {
                var blobKey = await blobService.UploadFileToBlobStorage(Guid.NewGuid(), data,
                    blobService.GetMimeType(filename), BlobServiceBase.FileType.AttachedDocument, DateTime.Now);

                if (blobKey == Guid.Empty)
                {
                    return;
                }

                var tmpFile = new AttachedDocument()
                {
                    AttachmentType = NomenclatureConstants.AttachedTypes.SignTempFile,
                    ParentId = 0,
                    BlobKey = blobKey,
                    FileName = filename,
                    CreateDate = DateTime.Now,
                    Gid = Guid.NewGuid(),
                    ModifyDate = DateTime.Now
                };
                await repo.AddAsync(tmpFile);
                await repo.SaveChangesAsync();
            }
            catch { }
        }
    }
}

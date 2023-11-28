using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Document;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.Core.Contracts
{
    public interface IDocumentService : IBaseService
    {
        Task<List<FileListVM>> GetFileList(Guid documentGid);
        Task<SaveResultVM> InitDocument(Guid? caseGid = null, Guid? sideGid = null, string description = null);
        Task<ElectronicDocumentVM> GetById(Guid gid);
        Task<SaveResultVM> RemoveFile(Guid gid);
        Task<SaveResultVM> RemoveSide(Guid gid);
        Task<SaveResultVM> SaveSideData(ElectronicDocumentSideVM model);
        Task<IQueryable<ElectronicDocumentSideVM>> SelectSides(Guid documentGid, Guid? sideGid);
        Task<SaveResultVM> UploadFile(Guid documentGid, IFormFile file);
        IQueryable<DocumentListVM> Select(FilterDocumentListVM filter);
        Task<SaveResultVM> SaveDocumentData(ElectronicDocumentVM model);
        Task<DocumentListVM> ReadForPrint(Guid gid);
        Task<SaveResultVM> SaveDocumentApply(Guid documentGid, byte[] pdfBytes);
        Task<SaveResultVM> SaveDocumentApplyTime(Guid documentGid);
        Task<SaveResultVM> CorrectDocument(Guid documentGid);
        Task<SaveResultVM> TestTimeStampt();
    }
}

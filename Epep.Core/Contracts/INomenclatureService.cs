using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.Contracts
{
    public interface INomenclatureService : IBaseService
    {
        Task<SaveResultVM> CourtSaveData(Court model);
        IQueryable<CourtListVM> CourtsSelect(FilterCourtVM filter);
        Task<List<SelectListItem>> GetDDL_ActKinds();
        Task<List<SelectListItem>> GetDDL_CaseKind();
        List<SelectListItem> GetDDL_CaseYears();
        Task<List<SelectListItem>> GetDDL_Courts(bool forElectronicDocument = false);
        Task<List<SelectListItem>> GetDDL_CourtsForDocument(int? documentKind);
        Task<List<SelectListItem>> GetDDL_CourtTypes();
        Task<List<SelectListItem>> GetDDL_ElectronicDocumentTypes(int? documentKind = null, long? courtId = null);
        Task<List<SelectListItem>> GetDDL_LawyerTypes();
        Task<List<SelectListItem>> GetDDL_SideInvolvementKind();
        List<SelectListItem> GetDDL_SubjectTypes();
        Task<string> GetInnerCode(string alias, string outerCode);
        string GetInnerCodeFromList(List<CodeMapping> mapList, string alias, string outerCode);
        Task<List<CourtListVM>> Get_CourtList();
    }
}

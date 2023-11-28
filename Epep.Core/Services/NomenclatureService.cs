using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.HSSF.Record;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class NomenclatureService : BaseService, INomenclatureService
    {
        public NomenclatureService(
            IRepository _repo,
            ILogger<NomenclatureService> _logger)
        {
            this.repo = _repo;
            this.logger = _logger;
        }

        public List<SelectListItem> GetDDL_CaseYears()
        {
            var result = new List<SelectListItem>();
            for (int year = DateTime.Now.Year; year >= 2010; year--)
            {
                result.Add(new SelectListItem(year.ToString(), year.ToString()));
            }

            return result;
        }


        public async Task<List<SelectListItem>> GetDDL_ActKinds()
        {
            return await repo.AllReadonly<ActKind>()
                            .Where(x => x.IsActive)
                            .OrderBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.ActKindId.ToString(),
                                Text = x.Name
                            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDL_CourtsForDocument(int? documentKind)
        {
            Expression<Func<Court, bool>> whereFilter = x => false;
            switch (documentKind)
            {
                case null:
                    whereFilter = x => true;
                    break;
                case NomenclatureConstants.DocumentKinds.Initial:
                    //whereFilter = x => (x.ForElectronicDocument == true) && (x.ForElectronicPayment == true);
                    //break;
                case NomenclatureConstants.DocumentKinds.SideDoc:
                case NomenclatureConstants.DocumentKinds.Compliant:
                    whereFilter = x => (x.ForElectronicDocument == true);
                    break;
                default:
                    break;
            }
            return await repo.AllReadonly<Court>()
                            .Where(x => x.IsActive)
                            .Where(whereFilter)
                            .OrderBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.CourtId.ToString(),
                                Text = x.Name
                            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDL_Courts(bool forElectronicDocument = false)
        {
            Expression<Func<Court, bool>> whereFilter = x => true;
            if (forElectronicDocument)
            {
                whereFilter = x => (x.ForElectronicDocument == true);
            }
            return await repo.AllReadonly<Court>()
                            .Where(x => x.IsActive)
                            .Where(whereFilter)
                            .OrderBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.CourtId.ToString(),
                                Text = x.Name
                            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDL_CourtTypes()
        {
            return await repo.AllReadonly<CourtType>()
                            .Where(x => x.IsActive)
                            .OrderBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.CourtTypeId.ToString(),
                                Text = x.Name
                            }).ToListAsync();
        }

        public async Task<List<CourtListVM>> Get_CourtList()
        {
            return await repo.AllReadonly<Court>()
                           .Where(x => x.IsActive)
                           .OrderBy(x => x.CourtTypeId)
                           .ThenBy(x => x.Name)
                           .Select(x => new CourtListVM
                           {
                               CourtTypeId = x.CourtTypeId,
                               CourtType = x.CourtType.Name,
                               CourtName = x.Name,
                               HasElectronicDocument = x.ForElectronicDocument == true,
                               HasElectronicPayment = x.ForElectronicPayment == true,
                               Url = x.Url
                           }).ToListAsync();
        }


        public async Task<List<SelectListItem>> GetDDL_CaseKind()
        {
            return await repo.AllReadonly<CaseKind>()
                            .Where(x => x.IsActive)
                            .OrderBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.CaseKindId.ToString(),
                                Text = x.Name
                            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDL_LawyerTypes()
        {
            return await repo.AllReadonly<LawyerType>()
                            .Where(x => x.IsActive)
                            .OrderBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.LawyerTypeId.ToString(),
                                Text = x.Name
                            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDL_ElectronicDocumentTypes(int? documentKind = null, long? courtId = null)
        {
            Expression<Func<ElectronicDocumentType, bool>> whereFilter = x => true;
            if (documentKind > 0)
            {
                whereFilter = x => (x.DocumentKind == documentKind);
            }

            Expression<Func<ElectronicDocumentType, bool>> wherePaymentsFilter = x => true;
            if (courtId > 0)
            {
                bool hasElPayments = (await repo.GetPropByIdAsync<Court, bool?>(x => x.CourtId == courtId, x => x.ForElectronicPayment) ?? false);
                if (!hasElPayments)
                {

                    long[] tarifDocuments = await repo.AllReadonly<MoneyPricelistDocument>()
                                                            .Where(x => x.Pricelist.DateTo == null || x.Pricelist.DateTo > dtNow)
                                                            .Select(x => x.ElectronicDocumentTypeId)
                                                            .Distinct()
                                                            .ToArrayAsync();
                    wherePaymentsFilter = x => !tarifDocuments.Contains(x.Id);
                }
            }

            return await repo.AllReadonly<ElectronicDocumentType>()
                            .Where(x => x.IsActive)
                            .Where(whereFilter)
                            .Where(wherePaymentsFilter)
                            .OrderBy(x => x.DocumentKind)
                            .ThenBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.Id.ToString(),
                                Text = x.Name + " (" + (x.DocumentKind == NomenclatureConstants.DocumentKinds.Initial ? "И" : ((x.DocumentKind == NomenclatureConstants.DocumentKinds.Compliant) ? "С" : "Д")) + ")"
                            }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDL_SideInvolvementKind()
        {
            return await repo.AllReadonly<SideInvolvementKind>()
                            .Where(x => x.IsActive)
                            .OrderBy(x => x.Name)
                            .Select(x => new SelectListItem
                            {
                                Value = x.SideInvolvementKindId.ToString(),
                                Text = x.Name
                            }).ToListAsync();
        }

        public List<SelectListItem> GetDDL_SubjectTypes()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem("Физическо лице",NomenclatureConstants.SubjectTypes.Person.ToString()),
                new SelectListItem("Юридическо лице",NomenclatureConstants.SubjectTypes.Entity.ToString())
            };
        }

        public IQueryable<CourtListVM> CourtsSelect(FilterCourtVM filter)
        {
            filter.Sanitize();
            Expression<Func<Court, bool>> whereName = x => true;
            if (!string.IsNullOrEmpty(filter.Name))
            {
                whereName = x => EF.Functions.Like(x.Name, filter.Name.ToPaternSearch());
            }
            Expression<Func<Court, bool>> whereType = x => true;
            if (filter.CourtTypeId > 0)
            {
                whereType = x => x.CourtTypeId == filter.CourtTypeId;
            }

            return repo.AllReadonly<Court>()
                           .Where(whereName)
                           .Where(whereType)
                           .OrderBy(x => x.CourtTypeId)
                           .ThenBy(x => x.Name)
                           .Select(x => new CourtListVM
                           {
                               CourtId = x.CourtId,
                               CourtTypeId = x.CourtTypeId,
                               CourtType = x.CourtType.Name,
                               CourtName = x.Name,
                               HasElectronicDocument = x.ForElectronicDocument == true,
                               HasElectronicPayment = x.ForElectronicPayment == true,
                               Url = x.Url,
                               isActive = x.IsActive
                           });
        }

        public async Task<SaveResultVM> CourtSaveData(Court model)
        {
            try
            {
                var saved = await repo.GetByIdAsync<Court>(model.CourtId);
                saved.IsActive = model.IsActive;
                saved.Name = model.Name;
                saved.IsIntegrated = model.IsIntegrated;
                saved.ForElectronicDocument = model.ForElectronicDocument;
                saved.ForElectronicPayment = model.ForElectronicPayment;
                saved.Url = model.Url;
                await repo.SaveChangesAsync();
                return new SaveResultVM(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return new SaveResultVM(false);
            }
        }

        public async Task<string> GetInnerCode(string alias, string outerCode)
        {
            return await repo.AllReadonly<CodeMapping>()
                            .Where(x => x.Alias == alias && x.OuterCode == outerCode)
                            .Select(x => x.InnerCode)
                            .FirstOrDefaultAsync();
        }

        public string GetInnerCodeFromList(List<CodeMapping> mapList, string alias, string outerCode)
        {
            return mapList
                    .Where(x => x.Alias == alias && x.OuterCode == outerCode)
                    .Select(x => x.InnerCode)
                    .FirstOrDefault();
        }
    }
}

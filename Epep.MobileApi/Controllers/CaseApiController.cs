using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.MobileApi.Extensions;
using Epep.MobileApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Epep.MobileApi.Controllers
{
    [Produces("application/json")]
    public class CaseApiController : BaseApiController
    {


        private readonly ILogger<CaseApiController> _logger;
        private readonly ICaseService caseService;
        private readonly DateTime UpgradeEpepDateStart;
        private readonly IUserContext userContext;
        private readonly string eCaseDownloadPath;
        public CaseApiController(ILogger<CaseApiController> logger, IConfiguration config, ICaseService _caseService, IUserContext _userContext)
        {
            _logger = logger;
            caseService = _caseService;
            userContext = _userContext;
            UpgradeEpepDateStart = config.GetValue<DateTime>("UpgradeEpepDateStart", DateTime.Now.AddMonths(-1));
            eCaseDownloadPath = config.GetValue<string>("DownloadPath", "https://mstest2.is-bg.net/epep-web/api/file/download/");
        }


        /// <summary>
        /// Връща списък дела по подадени критерии
        /// </summary>
        /// <param name="court">Код на съд</param>
        /// <param name="caseKind">Код на точен вид дело</param>
        /// <param name="regNumber">Номер дело</param>
        /// <param name="regYear">Година на делото</param>
        /// <param name="prevNumber">Номер на предходно дело</param>
        /// <param name="prevYear">Година на предходно дело</param>
        /// <param name="inNumber">Входящ номер на иницииращ документ</param>
        /// <param name="actKind">Код на вид акт</param>
        /// <param name="actNumber">Номер акт</param>
        /// <param name="actYear">Година на акта</param>
        /// <param name="page">Поредна страница</param>
        /// <param name="size">Размер на страницата</param>
        /// <returns></returns>
        [HttpGet("GetCases", Name = "GetCases")]

        public async Task<CaseListVM> GetCases(
            long? court = null, long? caseKind = null,
            int? regNumber = null, int? regYear = null,

            int? prevNumber = null, int? prevYear = null,
            int? inNumber = null,

            long? actKind = null, int? actNumber = null, int? actYear = null,
            int page = 1, int size = 20)
        {
            var filter = new FilterCaseVM()
            {
                CourtId = court,
                CaseKindId = caseKind,
                RegNumber = regNumber,
                RegYear = regYear,
                PrevNumber = prevNumber,
                PrevYear = prevYear,
                DocNumber = inNumber,
                ActKindId = actKind,
                ActNumber = actNumber,
                ActYear = actYear,
                MyCasesOnly = false
            };
            var data = await caseService.SelectCase(filter);
            int totalCount = data.SafeCount();
            var cases = data.Skip((page - 1) * size).Take(size).ToList();
            foreach (var item in cases)
            {
                caseService.ProcessCaseNames(item, UpgradeEpepDateStart, true, userContext.UserType, userContext.CourtId);
            }

            var result = new CaseListVM()
            {
                Items = cases,
                NextPageUrl = Url.Action(nameof(GetCases), "Case", new { court, caseKind, regNumber, regYear, prevNumber, prevYear, inNumber, actKind, actNumber, actYear, page = page + 1, size }, HttpContext.Request.Scheme)
            };
            if (totalCount < page * size)
            {
                result.NextPageUrl = null;
            }
            return result;
        }

        /// <summary>
        /// Връща списък от дела, до които имам достъп по подадени критерии
        /// </summary>
        /// <param name="court">Код на съд</param>
        /// <param name="caseKind">Код на точен вид дело</param>
        /// <param name="regNumber">Номер дело</param>
        /// <param name="regYear">Година на делото</param>
        /// <param name="prevNumber">Номер на предходно дело</param>
        /// <param name="prevYear">Година на предходно дело</param>
        /// <param name="inNumber">Входящ номер на иницииращ документ</param>
        /// <param name="actKind">Код на вид акт</param>
        /// <param name="actNumber">Номер акт</param>
        /// <param name="actYear">Година на акта</param>
        /// <param name="page">Поредна страница</param>
        /// <param name="size">Размер на страницата</param>
        /// <returns></returns>
        [HttpGet("GetMyCases", Name = "GetMyCases")]

        public async Task<CaseListVM> GetMyCases(
            long? court = null, long? caseKind = null,
            int? regNumber = null, int? regYear = null,

            int? prevNumber = null, int? prevYear = null,
            int? inNumber = null,

            long? actKind = null, int? actNumber = null, int? actYear = null,
            int page = 1, int size = 20)
        {
            var filter = new FilterCaseVM()
            {
                CourtId = court,
                CaseKindId = caseKind,
                RegNumber = regNumber,
                RegYear = regYear,
                PrevNumber = prevNumber,
                PrevYear = prevYear,
                DocNumber = inNumber,
                ActKindId = actKind,
                ActNumber = actNumber,
                ActYear = actYear,
                MyCasesOnly = true
            };
            var data = await caseService.SelectCase(filter);
            int totalCount = data.SafeCount();
            var cases = data.Skip((page - 1) * size).Take(size).ToList();

            var result = new CaseListVM()
            {
                Items = cases,
                NextPageUrl = Url.Action(nameof(GetCases), "Case", new { court, caseKind, regNumber, regYear, prevNumber, prevYear, inNumber, actKind, actNumber, actYear, page = page + 1, size }, HttpContext.Request.Scheme)
            };
            if (totalCount < page * size)
            {
                result.NextPageUrl = null;
            }
            return result;
        }

        [HttpGet("GetAssignments", Name = "GetAssignments")]
        public async Task<IEnumerable<AssignmentVM>> GetAssignments(Guid caseGid)
        {
            var loader = new GidLoaderVM()
            {
                Gid = caseGid
            };
            var data = await caseService.SelectAssignmentsByCaseGid(loader);
            return data.ToList();
        }

        [HttpGet("GetDocuments", Name = "GetDocuments")]
        public async Task<IEnumerable<DocumentVM>> GetDocuments(Guid caseGid)
        {
            var loader = new GidLoaderVM()
            {
                Gid = caseGid
            };
            var data = await caseService.SelectDocumentsByCaseGid(loader);
            return data.ToList();
        }

        [HttpGet("GetSides", Name = "GetSides")]
        public async Task<IEnumerable<SideVM>> GetSides(Guid caseGid)
        {
            var loader = new GidLoaderVM()
            {
                Gid = caseGid
            };
            var data = (await caseService.SelectSidesByCaseGid(loader)).ToList();

            return await caseService.ProcessSideNames(data, UpgradeEpepDateStart, caseGid, userContext.UserType, userContext.CourtId);
        }

        [HttpGet("GetHearings", Name = "GetHearings")]
        public async Task<IEnumerable<HearingVM>> GetHearings(Guid caseGid)
        {
            var loader = new GidLoaderVM()
            {
                Gid = caseGid
            };
            var data = await caseService.SelectHearingsByCaseGid(loader);
            return data.ToList();
        }

        [HttpGet("GetActs", Name = "GetActs")]
        public async Task<IEnumerable<ActVM>> GetActs(Guid? caseGid = null, Guid? hearingGid = null)
        {
            var loader = new GidLoaderVM()
            {
                ParentGid = hearingGid,
                Gid = caseGid ?? Guid.Empty
            };
            var data = await caseService.SelectActsByCaseGid(loader);
            return data.ToList();
        }

        [HttpGet("CaseDetails", Name = "CaseDetails")]
        public async Task<CaseApiVM> CaseDetails(Guid caseGid)
        {
            var list = await caseService.SelectCase(new FilterCaseVM()
            {
                CaseGid = caseGid
            });
            if (list == null)
            {
                return null;
            }
            CaseApiVM model = await list.FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }
            model.HasAccess = (await caseService.CheckCaseAccess(caseGid));
            return model;
        }

        [HttpGet("HearingDetails", Name = "HearingDetails")]
        public async Task<DetailsApiVM> HearingDetails(Guid gid)
        {
            var data = (await caseService.SelectHearingsByCaseGid(new GidLoaderVM()
            {
                ObjectGid = gid
            })).FirstOrDefault();
            if (data == null)
            {
                return null;
            }
            var result = new DetailsApiVM()
            {
                TypeName = data.HearingType,
                Date = data.Date
            };
            result.Participants = (await caseService.SelectHearingParticipantByHearing(new GidLoaderVM()
            {
                Gid = gid
            })).Select(x => new DetailsParticipants { FullName = x.FullName, Role = x.Role });
            result.CaseElements = (await caseService.SelectHearingItemsByHearing(new GidLoaderVM()
            {
                Gid = gid
            }));
            result.Files = resolveFilePath(await caseService.SelectHearingProtocolByHearingGid(data.Gid));
            return result;
        }

        [HttpGet("ActDetails", Name = "ActDetails")]
        public async Task<DetailsApiVM> ActDetails(Guid gid)
        {
            var model = await caseService.LoadFilesForObject(NomenclatureConstants.SourceTypes.Act, gid);
            if (model == null)
            {
                return null;
            }
            var result = new DetailsApiVM()
            {
                TypeName = model.TypeName,
                Number = model.Number,
                Date = model.Date,
                Participants = (await caseService.SelectHearingParticipantByHearing(new GidLoaderVM()
                {
                    Gid = gid
                })).Select(x => new DetailsParticipants { FullName = x.FullName, Role = x.Role }),
                Files = resolveFilePath(model.Files)
            };
            return result;
        }

        [HttpGet("DocumentDetails", Name = "DocumentDetails")]
        public async Task<DetailsApiVM> DocumentDetails(Guid gid, int type)
        {
            var model = await caseService.LoadFilesForObject(type, gid);
            if (model == null)
            {
                return null;
            }
            var result = new DetailsApiVM()
            {
                TypeName = model.TypeName,
                Number = model.Number,
                Date = model.Date,
                Files = resolveFilePath(model.Files)
            };
            return result;
        }

        [HttpGet("AssignmentDetails", Name = "AssignmentDetails")]
        public async Task<DetailsApiVM> AssignmentDetails(Guid gid)
        {
            var model = await caseService.LoadFilesForObject(NomenclatureConstants.SourceTypes.Assignment, gid);
            if (model == null)
            {
                return null;
            }
            var result = new DetailsApiVM()
            {
                TypeName = model.TypeName,
                Date = model.Date,
                Files = resolveFilePath(model.Files)
            };
            return result;
        }

        private List<FileItemApiVM> resolveFilePath(List<FileItemVM> files)
        {
            var result = new List<FileItemApiVM>();
            foreach (var item in files)
            {
                result.Add(new FileItemApiVM()
                {
                    FileGid = item.FileGid,
                    Title = item.Title,
                    Type = item.Type,
                    FileName = item.FileName,
                    FileUrl = $"{eCaseDownloadPath}{item.FileGid}"
                });
            }
            return result;
        }
    }
}
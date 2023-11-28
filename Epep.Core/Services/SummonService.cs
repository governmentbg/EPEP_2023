using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Case;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class SummonService : BaseService, ISummonService
    {
        private readonly IUserContext userContext;
        private readonly IBlobService blobService;
        public SummonService(
            IRepository _repo,
            IBlobService _blobService,
            ILogger<SummonService> _logger,
            IUserContext userContext)
        {
            this.repo = _repo;
            blobService = _blobService;
            this.logger = _logger;
            this.userContext = userContext;
        }

        public IQueryable<SummonVM> SelectSummonsByUser(FilterSummonVM filter)
        {
            filter.Sanitize();

            Expression<Func<Summon, bool>> whereCourt = x => true;
            if (filter.CourtId > 0)
            {
                whereCourt = x => x.Side.Case.CourtId == filter.CourtId;
            }

            Expression<Func<Summon, bool>> whereCaseKind = x => true;
            if (filter.CaseKindId > 0)
            {
                whereCaseKind = x => x.Side.Case.CaseKindId == filter.CaseKindId;
            }
            Expression<Func<Summon, bool>> whereRegNumber = x => true;
            if (filter.RegNumber > 0)
            {
                whereRegNumber = x => x.Side.Case.Number == filter.RegNumber;
            }
            Expression<Func<Summon, bool>> whereRegYear = x => true;
            if (filter.RegYear > 0)
            {
                whereRegYear = x => x.Side.Case.CaseYear == filter.RegYear;
            }
            Expression<Func<Summon, bool>> whereIsRead = x => true;
            if (filter.NotReadOnly)
            {
                whereIsRead = x => !x.IsRead;
            }

            Expression<Func<Summon, bool>> whereSummonKind = x => true;
            if (!string.IsNullOrEmpty(filter.SummonKind))
            {
                whereSummonKind = x => EF.Functions.Like(x.SummonKind, filter.SummonKind.ToPaternSearch());
            }

            Expression<Func<Summon, bool>> whereDateFrom = x => true;
            if (filter.DateFrom.HasValue)
            {
                whereDateFrom = x => x.CreateDate >= filter.DateFrom.Value;
            }
            Expression<Func<Summon, bool>> whereDateTo = x => true;
            if (filter.DateTo.HasValue)
            {
                whereDateTo = x => x.CreateDate <= filter.DateTo.MakeEndDate();
            }

            Expression<Func<Summon, bool>> whereNumber = x => true;
            if (!string.IsNullOrEmpty(filter.Number))
            {
                whereNumber = x => EF.Functions.Like(x.Number, filter.Number.ToPaternSearch());
            }


            Expression<Func<Summon, bool>> wherePerson = x => false;
            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                    wherePerson = x => x.Side.UserAssignments.Any(a => a.UserRegistrationId == userContext.OrganizationUserId && a.IsActive);
                    break;
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    wherePerson = x => x.Side.UserAssignments.Any(a => a.UserRegistrationId == userContext.OrganizationUserId && a.IsActive)
                    && x.Side.Case.OrganizationCases.Any(c => c.UserRegistrationId == userContext.UserId && c.NotificateUser && c.IsActive);
                    break;
                default:
                    wherePerson = x => x.Side.UserAssignments.Any(a => a.UserRegistrationId == userContext.UserId && a.IsActive);
                    break;
            }

            return repo.AllReadonly<Summon>()
                        .Where(wherePerson)
                        .Where(whereCourt)
                        .Where(whereCaseKind)
                        .Where(whereRegNumber)
                        .Where(whereRegYear)
                        .Where(whereIsRead)
                        .Where(whereSummonKind)
                        .Where(whereDateFrom)
                        .Where(whereDateTo)
                        .OrderByDescending(x => x.DateCreated)
                        .Select(x => new SummonVM
                        {
                            Gid = x.Gid,
                            SummonType = x.SummonType.Name,
                            SummonKind = x.SummonKind,
                            Number = x.Number,
                            Subject = x.Addressee,
                            DateCreated = x.DateCreated,
                            DateServed = x.DateServed,
                            IsRead = x.IsRead,
                            ReadTime = x.ReadTime,
                            CourtName = x.Side.Case.Court.Name,
                            CaseInfo = $"{x.Side.Case.CaseKind.Label} {x.Side.Case.Number}/{x.Side.Case.CaseYear}",
                            CaseGid = $"{x.Side.Case.Gid}"
                        }
                ).AsQueryable();
        }

        public async Task<SummonDocumentVM> GetSummonDocumentInfo(Guid gid)
        {
            var result = await repo.AllReadonly<Summon>()
                                    .Where(x => x.Gid == gid)
                                    .Select(x => new SummonDocumentVM
                                    {
                                        CourtName = x.Side.Case.Court.Name,
                                        CaseNumber = x.Side.Case.Number,
                                        CaseYear = x.Side.Case.CaseYear,
                                        CaseKind = x.Side.Case.CaseKind.Name,
                                        Addressee = x.Addressee,
                                        SummonKind = x.SummonKind,
                                        ReadTime = x.ReadTime ?? DateTime.MinValue,
                                        DateCreated = x.DateCreated,
                                        SummonBlobKey = x.SummonBlobKey
                                    }).FirstOrDefaultAsync();

            if (result.SummonBlobKey.HasValue)
            {
                var blobInfo = await blobService.GetBlobInfo(result.SummonBlobKey.Value);
                if (blobInfo != null)
                    result.SummonContent = await blobService.GetFileContent(blobInfo.BlobKey);
            }

            return result;
        }

        public async Task<bool> SetSummonAsRead(Guid gid, DateTime readTime, byte[] reportBytes, byte[] timestampBytes)
        {


            var summon = await GetByGidAsync<Summon>(gid);
            if (summon.IsRead)
            {
                return false;
            }

            var reportBlobKey = await blobService.UploadFileToBlobStorage(Guid.NewGuid(), reportBytes,
                blobService.GetMimeType("1.pdf"), BlobServiceBase.FileType.SummonReport, readTime);

            if (reportBlobKey == Guid.Empty)
            {
                return false;
            }



            if (timestampBytes != null)
            {
                Guid reportTimestampKey = await blobService.UploadFileToBlobStorage(Guid.NewGuid(), timestampBytes,
                   blobService.GetMimeType("1.tsr"), BlobServiceBase.FileType.SummonTimeStamp, readTime);

                if (reportTimestampKey != Guid.Empty)
                {
                    summon.ReportReadTimeBlobKey = reportTimestampKey;
                }
            }
            summon.IsRead = true;
            summon.ReadTime = readTime;
            summon.ReportBlobKey = reportBlobKey;
            summon.ModifyDate = DateTime.Now;

            await repo.SaveChangesAsync();

            return true;
        }
    }
}

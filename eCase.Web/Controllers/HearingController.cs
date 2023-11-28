using System;
using System.Linq;
using System.Web.Mvc;

using eCase.Common.Enums;
using eCase.Common.Helpers;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using eCase.Web.Helpers;
using eCase.Web.Models.Hearing;

using PagedList;
using System.Data.Entity;

namespace eCase.Web.Controllers
{
    public partial class HearingController : BaseController
    {
        public HearingController(
            IHearingRepository hearingRepository,
            IHearingDocumentRepository hearingDocumentRepository,
            IAttachedDocumentRepository attachedDocumentRepository,
            ISummonRepository summonRepository)
            : base(summonRepository)
        {
            _hearingRepository = hearingRepository;
            _hearingDocumentRepository = hearingDocumentRepository;
            _attachedDocumentRepository = attachedDocumentRepository;
        }

        [HttpGet]
        public virtual ActionResult Details(Guid gid, int? sPage)
        {
            HearingDetailsVM vm = new HearingDetailsVM();

            vm.sPage = sPage;

            Hearing hearing = _hearingRepository.SetWithoutIncludes().Where(e => e.Gid == gid)
                .Include(e => e.Case)
                .Include(e => e.Case.Court)
                .Include(e => e.HearingParticipants)
                .Include(e => e.Acts)
                .Include(e => e.Acts.Select(a => a.ActKind))
                .Include(e => e.Acts.Select(a => a.ActPreparators))
                .Include(e => e.HearingDocuments)
                .Include(e => e.HearingDocuments.Select(a => a.Side.Subject))
                .Single();

            vm.Hearing = hearing;

            vm.HasPermissions = Request.IsAuthenticated && _hearingRepository.CheckPermission(gid, CurrentUser.UserID);

            if (vm.HasPermissions)
            {
                vm.HasProtocolFile = hearing.PrivateBlobKey.HasValue;
                vm.HearingDocuments = hearing.HearingDocuments.ToList();
                vm.AttachedDocument = _attachedDocumentRepository.GetAttachedDocuments(AttachedTypes.SessionFastDocument, vm.HearingDocuments.Select(d => d.HearingDocumentId).ToArray()).ToList();
            }
            else
            {
                vm.HasProtocolFile = hearing.PublicBlobKey.HasValue;
            }

            if (ShowSummons)
            {
                vm.Summons = _summonRepository.GetSummonsByUserByType(CurrentUser.UserID, SummonTypeNomenclature.Hearing, hearing.HearingId).ToList().ToPagedList(vm.sPage ?? 1, Statics.SmallPageSize);
            }

            return View(vm);
        }

        #region Files

        [HttpGet]
        public virtual RedirectResult GetProtocolFile(Guid hearingGid)
        {
            Hearing hearing = _hearingRepository.FindByGid(hearingGid);
            if (hearing == null)
                return null;

            Guid blobKey = Guid.Empty;

            // Check permissions
            if (Request.IsAuthenticated && _hearingRepository.CheckPermission(hearingGid, CurrentUser.UserID))
            {
                var privateBlob = hearing.PrivateBlobKey;

                if (privateBlob == null)
                    return null;

                blobKey = privateBlob.Value;
            }
            else
            {
                var publicBlob = hearing.PublicBlobKey;

                if (publicBlob == null)
                    return null;

                blobKey = publicBlob.Value;
            }

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        #endregion

        #region Private

        private IHearingRepository _hearingRepository;
        private IHearingDocumentRepository _hearingDocumentRepository;
        private IAttachedDocumentRepository _attachedDocumentRepository;

        #endregion
    }
}
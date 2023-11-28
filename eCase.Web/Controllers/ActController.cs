using System;
using System.Linq;
using System.Web.Mvc;

using eCase.Common.Enums;
using eCase.Common.Helpers;
using eCase.Data.Repositories;
using eCase.Domain.Entities;
using eCase.Web.Helpers;
using eCase.Web.Models.Act;

using PagedList;
using System.Data.Entity;
using System.Collections.Generic;

namespace eCase.Web.Controllers
{
    public partial class ActController : BaseController
    {
        public ActController(
            IActRepository actRepository,
            IAttachedDocumentRepository attachedDocumentRepository,
            ISummonRepository summonRepository)
            : base(summonRepository)
        {
            _actRepository = actRepository;
            _attachedDocumentRepository = attachedDocumentRepository;
        }

        [HttpGet]
        public virtual ActionResult Details(Guid gid, int? sPage)
        {
            ActDetailsVM vm = new ActDetailsVM();

            vm.sPage = sPage;

            Act act = _actRepository.SetWithoutIncludes().Where(e => e.Gid == gid)
                .Include(e => e.Case)
                .Include(e => e.Case.Court)
                .Include(e => e.Case.CaseType)
                .Include(e => e.ActKind)
                .Include(e => e.ActPreparators)
                .Single();

            vm.Act = act;
            vm.IsCriminal = CaseTypeNomenclature.Criminal.Code == act.Case.CaseType.Code;
            vm.HasPermissions = Request.IsAuthenticated && _actRepository.CheckPermission(gid, CurrentUser.UserID);

            if (vm.HasPermissions)
            {
                vm.HasActFile = act.PrivateActBlobKey.HasValue;
                vm.HasMotiveFile = act.PrivateMotiveBlobKey.HasValue;
            }
            else
            {
                //vm.HasActFile = act.PublicActBlobKey.HasValue;
                //vm.HasMotiveFile = act.PublicMotiveBlobKey.HasValue;
            }

            vm.HasPublicActFile = act.PublicActBlobKey.HasValue;
            vm.HasPublicMotiveFile = act.PublicMotiveBlobKey.HasValue;

            if (ShowSummons)
            {
                vm.Summons = _summonRepository.GetSummonsByUserByType(CurrentUser.UserID, SummonTypeNomenclature.Act, act.ActId).ToList().ToPagedList(vm.sPage ?? 1, Statics.SmallPageSize);
            }

            #region AttachedDocuments
            vm.AttachedDocument = new List<AttachedDocument>();
            if (vm.HasPermissions)
            {
                vm.AttachedDocument.AddRange(_attachedDocumentRepository.GetAttachedDocuments(AttachedTypes.ActCoordination, act.ActId));
            }
            vm.AttachedDocument.AddRange(_attachedDocumentRepository.GetAttachedDocuments(AttachedTypes.ActCoordinationPublic, act.ActId));
            #endregion

            return View(vm);
        }

        #region Files

        [HttpGet]
        public virtual RedirectResult GetActFile(Guid actGid, bool getPublicFile = false)
        {
            Act act = _actRepository.FindByGid(actGid);
            if (act == null)
                return null;

            Guid blobKey = Guid.Empty;

            // Check permissions
            if (!getPublicFile && Request.IsAuthenticated && _actRepository.CheckPermission(actGid, CurrentUser.UserID))
            {
                var privateBlob = act.PrivateActBlobKey;

                if (privateBlob == null)
                    return null;

                blobKey = privateBlob.Value;
            }
            else
            {
                var publicBlob = act.PublicActBlobKey;

                if (publicBlob == null)
                    return null;

                blobKey = publicBlob.Value;
            }

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        [HttpGet]
        public virtual RedirectResult GetActPublicFile(Guid guid)
        {
            Act act = _actRepository.FindByGid(guid);
            if (act == null)
                return null;

            var publicBlob = act.PublicActBlobKey;

            if (publicBlob == null)
                return null;

            Guid blobKey = publicBlob.Value;

            return Redirect(Constants.DownloadUrl + blobKey.ToString());
        }

        [HttpGet]
        public virtual RedirectResult GetMotiveFile(Guid actGid, bool getPublicFile = false)
        {
            Act act = _actRepository.FindByGid(actGid);
            if (act == null)
                return null;

            Guid blobKey = Guid.Empty;

            // Check permissions
            if (!getPublicFile && Request.IsAuthenticated && _actRepository.CheckPermission(actGid, CurrentUser.UserID))
            {
                var privateBlob = act.PrivateMotiveBlobKey;

                if (privateBlob == null)
                    return null;

                blobKey = privateBlob.Value;
            }
            else
            {
                var publicBlob = act.PublicMotiveBlobKey;

                if (publicBlob == null)
                    return null;

                blobKey = publicBlob.Value;
            }

            return Redirect(Constants.DownloadUrl + blobKey);
        }

        [HttpGet]
        public virtual RedirectResult GetMotivePublicFile(Guid guid)
        {
            Act act = _actRepository.FindByGid(guid);
            if (act == null)
                return null;

            var publicBlob = act.PublicMotiveBlobKey;

            if (publicBlob == null)
                return null;

            Guid blobKey = publicBlob.Value;

            return Redirect(Constants.DownloadUrl + blobKey.ToString());
        }

        #endregion

        #region Private

        private IActRepository _actRepository;
        private IAttachedDocumentRepository _attachedDocumentRepository;

        #endregion
    }
}
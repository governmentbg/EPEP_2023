using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IUserContext userContext;
        private readonly IBlobService blobService;

        public UserService(
            IRepository _repo,
            ILogger<UserService> _logger,
            IUserContext userContext,
            IBlobService blobService)
        {
            this.repo = _repo;
            this.logger = _logger;
            this.userContext = userContext;
            this.blobService = blobService;
        }

        public async Task<UserRegistration> FindByUIC(int loginUserType, string uic, string organizationUic = null)
        {
            Expression<Func<UserRegistration, bool>> whereUIC = x => false;
            switch (loginUserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                case NomenclatureConstants.UserTypes.Lawyer:
                    whereUIC = x => x.EGN == uic && x.UserTypeId == NomenclatureConstants.UserTypes.Person;
                    break;
                case NomenclatureConstants.UserTypes.Organization:
                    whereUIC = x => NomenclatureConstants.UserTypes.OrganizationTypes.Contains(x.UserTypeId)
                                    && x.EGN == uic
                                    && x.OrganizationUserId > 0
                                    && x.OrganizationRegistration.UIC == organizationUic
                                    && x.OrganizationRegistration.ComfirmedDate != null
                                    && x.ComfirmedDate != null;
                    break;
                case NomenclatureConstants.UserTypes.Administrator:
                    whereUIC = x => x.EGN == uic && NomenclatureConstants.UserTypes.AdministratorTypes.Contains(x.UserTypeId);
                    break;
                default:
                    break;
            }


            var result = await repo.AllReadonly<UserRegistration>()
                            .Where(whereUIC)
                            .Where(x => x.IsActive)
                            .FirstOrDefaultAsync();

            if (loginUserType == NomenclatureConstants.UserTypes.Lawyer && result != null)
            {
                var lawyerModel = await repo.AllReadonly<Lawyer>()
                                            .Where(x => x.Uic == uic)
                                            .Where(x => NomenclatureConstants.LawyerStates.LoginStates.Contains(x.LawyerStateId))
                                            .FirstOrDefaultAsync();
                if (lawyerModel == null)
                {
                    return null;
                }
                result.FullName = lawyerModel.Name;
            }

            return result;
        }

        public async Task<SaveResultVM> ValidateUserRegistration(UserRegistrationVM model)
        {
            var result = new SaveResultVM();
            if (model == null)
            {
                return result;
            }
            model.Sanitize();

            if (string.IsNullOrEmpty(model.Email))
            {
                result.AddError("Полето е задължително.", nameof(UserRegistrationVM.Email));
            }
            else
            {
                if (await repo.AllReadonly<UserRegistration>()
                                               .Where(x => x.UserTypeId == model.UserType)
                                               .Where(x => x.Email == model.Email)
                                               .Where(x => x.DeniedDate == null)
                                               .Where(x => x.IsActive)
                                               .AnyAsync())
                {
                    result.AddError("Съществува потребител с идентична електронна поща.", nameof(UserRegistrationVM.Email));
                }
            }
            switch (model.UserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                    model.UIC = null;
                    model.OrgFullName = null;
                    if (string.IsNullOrEmpty(model.FullName))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.FullName));
                    }
                    if (string.IsNullOrEmpty(model.EGN))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.EGN));
                    }
                    else
                    {
                        if (await repo.AllReadonly<UserRegistration>()
                                                        .Where(x => x.UserTypeId == model.UserType)
                                                        .Where(x => x.IsActive)
                                                        .Where(x => x.EGN == model.EGN)
                                                        .AnyAsync())
                        {
                            result.AddError("Съществува потребител с идентичен идентификатор.", nameof(UserRegistrationVM.EGN));
                        }
                    }


                    break;
                case NomenclatureConstants.UserTypes.Organization:
                    model.EGN = null;
                    model.FullName = null;

                    if (string.IsNullOrEmpty(model.UIC))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.UIC));
                    }

                    if (string.IsNullOrEmpty(model.RepresentativeFullName))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.RepresentativeFullName));
                    }
                    if (string.IsNullOrEmpty(model.RepresentativeEGN))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.RepresentativeEGN));
                    }
                    if (string.IsNullOrEmpty(model.RepresentativeEmail))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.RepresentativeEmail));
                    }

                    var exsitingOrganization = await repo.AllReadonly<UserRegistration>()
                                                    .Where(x => x.UserTypeId == model.UserType)
                                                    .Where(x => x.UIC == model.UIC)
                                                    .Where(x => x.IsActive)
                                                    .Where(x => x.DeniedDate == null)
                                                    .FirstOrDefaultAsync();
                    if (exsitingOrganization != null)
                    {
                        if (await repo.AllReadonly<UserRegistration>()
                                                        .Where(x => NomenclatureConstants.UserTypes.OrganizationUserTypes.Contains(x.UserTypeId))
                                                        .Where(x => x.EGN == model.RepresentativeEGN)
                                                        .Where(x => x.OrganizationUserId == exsitingOrganization.Id)
                                                        .Where(x => x.IsActive)
                                                        .Where(x => x.DeniedDate == null)
                                                        .AnyAsync())
                        {

                            result.AddError($"За организация {exsitingOrganization.FullName} съществува лице с идентичeн идентификатор.", nameof(UserRegistrationVM.RepresentativeEGN));
                        }
                        else
                        {
                            model.ExistingOrganizationId = exsitingOrganization.Id;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(model.OrgFullName))
                        {
                            result.AddError("Полето е задължително.", nameof(UserRegistrationVM.OrgFullName));
                        }
                        if (string.IsNullOrEmpty(model.Email))
                        {
                            result.AddError("Полето е задължително.", nameof(UserRegistrationVM.Email));
                        }
                    }
                    break;
                case NomenclatureConstants.UserTypes.Administrator:
                case NomenclatureConstants.UserTypes.CourtAdmin:
                case NomenclatureConstants.UserTypes.GlobalAdmin:
                    if (string.IsNullOrEmpty(model.FullName))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.FullName));
                    }
                    if (string.IsNullOrEmpty(model.EGN))
                    {
                        result.AddError("Полето е задължително.", nameof(UserRegistrationVM.EGN));
                    }
                    else
                    {
                        if (await repo.AllReadonly<UserRegistration>()
                                                        .Where(x => NomenclatureConstants.UserTypes.AdministratorTypes.Contains(x.UserTypeId))
                                                        .Where(x => x.IsActive)
                                                        .Where(x => x.EGN == model.EGN)
                                                        .AnyAsync())
                        {
                            result.AddError("Съществува потребител с идентичен идентификатор.", nameof(UserRegistrationVM.EGN));
                        }
                    }
                    break;
                default:
                    result.AddError("Навалидно подадени данни.");
                    break;
            }

            return result;
        }

        public async Task<UserRegistrationVM> SelectDataForProfile(Guid? gid = null)
        {
            Expression<Func<UserRegistration, bool>> whereSelect = x => x.Id == userContext.UserId;
            if (gid.HasValue)
            {
                whereSelect = x => x.Gid == gid.Value;
            }

            return await repo.AllReadonly<UserRegistration>()
                                .Where(whereSelect)
                                .Select(x => new UserRegistrationVM
                                {
                                    Gid = x.Gid,
                                    UserType = x.UserTypeId,
                                    UserTypeName = x.UserRegistrationType.Name,
                                    FullName = x.FullName,
                                    EGN = x.EGN,
                                    Email = x.Email,
                                    NotificationEmail = x.NotificationEmail,
                                    RegCertificateInfo = x.RegCertificateInfo,
                                    IsActive = x.IsActive,
                                    IsComfirmed = x.ComfirmedDate != null,
                                    OrgFullName = (x.OrganizationUserId > 0) ? x.OrganizationRegistration.FullName : "",
                                    UIC = (x.OrganizationUserId > 0) ? x.OrganizationRegistration.UIC : x.UIC,
                                    CourtId = x.CourtId,
                                    CourtName = (x.CourtId > 0) ? x.Court.Name : null
                                }).FirstOrDefaultAsync();
        }

        public async Task<UserComfirmedInfoVM> SelectConfirmInfo(Guid gid)
        {
            var result = await repo.AllReadonly<UserRegistration>()
                               .Where(x => x.Gid == gid)
                               .Select(x => new UserComfirmedInfoVM
                               {
                                   Id = x.Id,
                                   ComfirmedDate = x.ComfirmedDate,
                                   DeniedDate = x.DeniedDate,
                                   DeniedDescription = x.DeniedDescription,
                                   ComfirmedUserName = (x.ComfirmedUserId > 0) ? x.ComfirmedUser.FullName : "",
                                   ComfirmedUserType = (x.ComfirmedUserId > 0) ? x.ComfirmedUser.UserRegistrationType.Name : ""
                               }).FirstOrDefaultAsync();

            result.Files = await blobService.SelectAttachedDocument(NomenclatureConstants.AttachedTypes.UserRegistration, result.Id).ToListAsync();
            result.Files.AddRange(await blobService.SelectAttachedDocument(NomenclatureConstants.AttachedTypes.UserRegistrationRegixReport, result.Id).ToListAsync());

            return result;
        }

        public async Task<SaveResultVM> SaveRegistrationForm(byte[] pdfDoc, long userId)
        {
            var fileName = "userRegistration.pdf";
            var blobKey = await blobService.UploadFileToBlobStorage(Guid.NewGuid(), pdfDoc, blobService.GetMimeType(fileName), Core.Services.BlobServiceBase.FileType.AttachedDocument, DateTime.Now);
            if (blobKey == Guid.Empty)
            {
                return new SaveResultVM(false);
            }

            var newDoc = new AttachedDocument()
            {
                Gid = Guid.NewGuid(),
                AttachmentType = NomenclatureConstants.AttachedTypes.UserRegistration,
                ParentId = userId,
                BlobKey = blobKey,
                FileName = fileName,
                FileTitle = "Заявление за регистрация",
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now
            };

            await repo.AddAsync(newDoc);
            await repo.SaveChangesAsync();
            return new SaveResultVM(true);
        }

        public async Task<List<SelectListItem>> GetDDL_UserRegistrationTypes(bool adminsOnly = false)
        {
            return await repo.AllReadonly<UserRegistrationType>()
                                    .Where(x => x.IsActive)
                                    .Select(x => new SelectListItem
                                    {
                                        Value = x.Id.ToString(),
                                        Text = x.Name
                                    }).ToListAsync();
        }

        public async Task<List<SelectListItem>> GetDDL_UserVacationTypes()
        {
            return await repo.AllReadonly<UserVacationType>()
                                    .Where(x => x.IsActive)
                                    .Select(x => new SelectListItem
                                    {
                                        Value = x.Id.ToString(),
                                        Text = x.Name
                                    }).ToListAsync();
        }

        public IQueryable<UserVacationVM> UserVacation_Select()
        {
            return repo.AllReadonly<UserVacation>()
                        .Where(x => x.UserId == userContext.UserId)
                        .Where(x => x.DateExpire == null)
                        .OrderByDescending(x => x.DateFrom)
                        .Select(x => new UserVacationVM
                        {
                            Id = x.Id,
                            VacationType = x.VacationType.Name,
                            DateFrom = x.DateFrom,
                            DateTo = x.DateTo,
                            DocumentNumber = x.DocumentNumber
                        });
        }

        public async Task<SaveResultVM> UserVacation_SaveData(UserVacation model)
        {
            var result = await userVacation_Validate(model);
            if (!result.Result)
            {
                return result;
            }

            if (model.Id > 0)
            {
                var saved = await repo.GetByIdAsync<UserVacation>(model.Id);
                if (saved.UserId != userContext.UserId)
                {
                    result.AddError("Непозволена операция.");
                    return result;
                }

                saved.VacationTypeId = model.VacationTypeId;
                saved.DateFrom = model.DateFrom;
                saved.DateTo = model.DateTo.MakeEndDate();
                saved.DocumentNumber = model.DocumentNumber;
                saved.DateWrt = DateTime.Now;
                await repo.SaveChangesAsync();
            }
            else
            {
                model.UserId = userContext.UserId;
                model.DateWrt = DateTime.Now;
                await repo.AddAsync(model);
                await repo.SaveChangesAsync();
            }

            return result;
        }

        async Task<SaveResultVM> userVacation_Validate(UserVacation model)
        {
            var result = new SaveResultVM(true);
            if (model.VacationTypeId == NomenclatureConstants.UserVacationTypes.Ill)
            {
                if (string.IsNullOrEmpty(model.DocumentNumber))
                {
                    result.AddError("Полето е задължително.", nameof(model.DocumentNumber));
                }
            }

            if (await UserVacation_CalcDays(model.DateFrom.Year, model) > NomenclatureConstants.UserVacation_MaxYearDays)
            {
                result.AddError($"Надвишен лимит отсъствия за {model.DateFrom.Year}г.", nameof(model.DateFrom));
            }
            if (model.DateFrom.Year != model.DateTo.Year)
            {
                if (await UserVacation_CalcDays(model.DateTo.Year, model) > NomenclatureConstants.UserVacation_MaxYearDays)
                {
                    result.AddError($"Надвишен лимит отсъствия за {model.DateTo.Year}г.", nameof(model.DateTo));
                }
            }
            var hasOverlap = await repo.AllReadonly<UserVacation>()
                          .Where(x => x.UserId == userContext.UserId && x.Id != model.Id)
                          .Where(x => x.DateFrom <= model.DateTo && x.DateTo >= model.DateFrom)
                          .Where(x => x.DateExpire == null)
                          .AnyAsync();

            if (hasOverlap)
            {
                result.AddError($"Вече има отсъствия за въведения период", nameof(model.DateFrom));
            }
            if (model.DateFrom.Date < DateTime.Now.Date)
            {
                result.AddError($"Не може да въвеждат и редактират отсъствия за минал период.", nameof(model.DateFrom));
            }

            return result;
        }

        public async Task<int> UserVacation_CalcDays(int year, UserVacation current = null)
        {
            long currentId = 0;
            if (current != null)
            {
                currentId = current.Id;
            }
            var yearStart = new DateTime(year, 1, 1);
            var yearEnd = new DateTime(year, 12, 31);
            var saved = await repo.AllReadonly<UserVacation>()
                            .Where(x => x.UserId == userContext.UserId && x.Id != currentId)
                            .Where(x => x.DateFrom <= yearEnd && x.DateTo >= yearStart)
                            .Where(x => x.DateExpire == null)
                            .ToListAsync();

            if (current != null
                && current.DateFrom <= yearEnd && current.DateTo >= yearStart)
            {
                saved.Add(current);
            }

            int countDays = 0;
            foreach (var vacation in saved)
            {
                TimeSpan ts = minDate(vacation.DateTo, yearEnd) - maxDate(vacation.DateFrom, yearStart);
                countDays += (int)Math.Floor(ts.TotalDays);
            }
            return countDays;
        }

        public async Task<string> GetAudiInfo_User(UserRegistration saved, UserRegistrationVM updated)
        {
            var uType = (await repo.GetByIdAsync<UserRegistrationType>(updated.UserType)).Name;

            string result = "";
            if (saved == null)
            {
                result += formatAuditRow("Вид", uType);
                result += formatAuditRow("Имена", updated.FullName);
                result += formatAuditRow("Идентификатор", updated.EGN);
                result += formatAuditRow("Ел.поща", updated.Email);
            }
            else
            {

                if (updated.CourtId > 0 && saved.CourtId > 0)
                    if (updated.CourtId != saved.CourtId)
                    {
                        var oldCourt = await repo.GetPropByIdAsync<Court, string>(x => x.CourtId == saved.CourtId, x => x.Name);
                        var newCourt = await repo.GetPropByIdAsync<Court, string>(x => x.CourtId == updated.CourtId, x => x.Name);
                        result += formatAuditRow("Съд", newCourt, oldCourt, true);
                    }
                if (updated.FullName != saved.FullName)
                {
                    result += formatAuditRow("Имена", updated.FullName, saved.FullName, true);
                }
                if (updated.EGN != saved.EGN)
                {
                    result += formatAuditRow("Идентификатор", updated.EGN, saved.EGN, true);
                }
                if (updated.Email != saved.Email)
                {
                    result += formatAuditRow("Ел.поща", updated.Email, saved.Email, true);
                }
            }
            return result;
        }

        DateTime minDate(DateTime date1, DateTime date2)
        {
            if (date1 < date2)
            {
                return date1;
            }
            return date2;
        }
        DateTime maxDate(DateTime date1, DateTime date2)
        {
            if (date1 > date2)
            {
                return date1;
            }
            return date2;
        }

        public async Task<UserAccessVM> ManageAccessForSide(Guid sideGid)
        {
            var model = new UserAccessVM();

            var sideModel = await repo.AllReadonly<Side>()
                                        .Where(x => x.Gid == sideGid)
                                        .Select(x => new
                                        {
                                            CaseGid = x.Case.Gid,
                                            x.Case.RestrictedAccess,
                                            SideName = x.Subject.Name,
                                            x.Subject.SubjectTypeId,
                                            SideRole = x.SideInvolvementKind.Name
                                        }).FirstOrDefaultAsync();
            model.CaseGid = sideModel.CaseGid;
            model.SideGid = sideGid;
            model.SideName = sideModel.SideName;
            model.SideRole = sideModel.SideRole;
            if (sideModel.SubjectTypeId == NomenclatureConstants.SubjectTypes.Person && NomenclatureConstants.UserTypes.PersonTypes.Contains(userContext.UserType))
            {
                model.SideName = model.SideName.ToInitials();
            }
            if (userContext.UserType == NomenclatureConstants.UserTypes.Lawyer && (sideModel.RestrictedAccess == false))
            {
                model.SideName = sideModel.SideName;
            }
            var accessInfo = await repo.AllReadonly<UserAssignment>()
                                        .Where(x => x.Side.Gid == sideGid)
                                        .Where(x => x.UserRegistrationId == userContext.AccessUserId)
                                        .Select(x => new
                                        {
                                            x.Gid,
                                            CaseGid = x.Case.Gid,
                                            x.IsActive
                                        }).FirstOrDefaultAsync();

            if (accessInfo != null)
            {
                model.HasAccess = accessInfo.IsActive;
                model.RequestAccess = NomenclatureConstants.UserAccessModes.RequestAccess;
            }

            return model;
        }


        public async Task<string> ManageAccessGetDescription(UserAccessVM model)
        {
            var sideModel = await repo.AllReadonly<Side>()
                                        .Where(x => x.Gid == model.SideGid)
                                        .Select(x => new
                                        {
                                            CaseGid = x.Case.Gid,
                                            SideName = x.Subject.Name,
                                            x.Subject.SubjectTypeId,
                                            x.Case.RestrictedAccess,
                                            SideRole = x.SideInvolvementKind.Name
                                        }).FirstOrDefaultAsync();

            var accessInfo = await repo.AllReadonly<UserAssignment>()
                                      .Where(x => x.Side.Gid == model.SideGid)
                                      .Where(x => x.UserRegistrationId == userContext.AccessUserId)
                                      .Where(x => x.IsActive)
                                      .Select(x => new
                                      {
                                          x.Gid,
                                          CaseGid = x.Case.Gid
                                      }).FirstOrDefaultAsync();

            var userTypeName = "";
            var sideName = sideModel.SideName;
            if (sideModel.SubjectTypeId == NomenclatureConstants.SubjectTypes.Person)
            {
                sideName = sideModel.SideName.ToInitials();
            }
            switch (userContext.UserType)
            {
                case NomenclatureConstants.UserTypes.Person:
                    userTypeName = "физическо лице";
                    break;
                case NomenclatureConstants.UserTypes.Lawyer:
                    userTypeName = "адвокат";
                    if (sideModel.RestrictedAccess == false)
                    {
                        sideName = sideModel.SideName;
                    }
                    break;
                case NomenclatureConstants.UserTypes.OrganizationRepresentative:
                case NomenclatureConstants.UserTypes.OrganizationUser:
                    userTypeName = "юридическо лице";
                    break;
                default:
                    break;
            }


            switch (model.RequestAccess)
            {
                case NomenclatureConstants.UserAccessModes.RequestAccess:
                    var result = "";
                    //Ако все още няма достъп 
                    if (accessInfo == null)
                    {
                        result += $"Моля, да ми бъде предоставен електронен достъп до делото, като {userTypeName} чрез страна {sideName} в качеството на {sideModel.SideRole}.";
                    }

                    switch (model.RequestSummon)
                    {
                        case NomenclatureConstants.UserAccessModes.RequestSummon:
                            result += $"Желая, да бъда известяван(а) електронно за постъпили съобщения и призовки в хода на делото.";
                            break;
                        case NomenclatureConstants.UserAccessModes.RemoveSummon:
                            result += $"Отказвам, да бъда известяван(а) електронно за постъпили съобщения и призовки в хода на делото.";
                            break;
                    }
                    return result;

                case NomenclatureConstants.UserAccessModes.RemoveAccess:
                    return $"Моля, да бъде преустановен електронния ми достъп до делото, до страна {sideModel.SideName} в качеството на {sideModel.SideRole}.";
            }
            return string.Empty;
        }
    }
}

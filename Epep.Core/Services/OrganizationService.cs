using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class OrganizationService : BaseService, IOrganizationService
    {
        private readonly IUserContext userContext;
        public OrganizationService(
            IRepository _repo,
            ILogger<OrganizationService> _logger,
            IUserContext _userContext
            )
        {
            this.repo = _repo;
            this.logger = _logger;
            userContext = _userContext;
        }



        public async Task<OrganizationUserVM> SelectOrganizationUsersByGid(Guid gid)
        {
            return await repo.AllReadonly<UserOrganizationCase>()
                                  .Where(x => x.Gid == gid)
                                  .Select(x => new OrganizationUserVM
                                  {
                                      OrganizationUserGid = x.Gid,
                                      CaseGid = x.Case.Gid,
                                      UserGid = x.UserRegistration.Gid,
                                      FullName = x.UserRegistration.FullName,
                                      NotificateUser = x.NotificateUser,
                                      UserType = x.UserRegistration.UserTypeId,
                                      IsActive = x.IsActive
                                  }).FirstOrDefaultAsync();
        }
        public async Task<IQueryable<OrganizationUserVM>> SelectOrganizationUsersByCase(GidLoaderVM loader)
        {
            if (userContext.UserType != NomenclatureConstants.UserTypes.OrganizationRepresentative)
            {
                return null;
            }

            var caseModel = await GetByGidAsync<Case>(loader.Gid);
            if (caseModel == null)
            {
                return null;
            }


            var representatives = repo.AllReadonly<UserRegistration>()
                                    .Where(x => x.OrganizationUserId == userContext.OrganizationUserId)
                                    .Where(x => x.OrganizationRegistration.Assignments.Any(a => a.CaseId == caseModel.CaseId && a.IsActive))
                                    .Where(x => x.ComfirmedDate != null && x.IsActive)
                                    .Where(x => x.UserTypeId == NomenclatureConstants.UserTypes.OrganizationRepresentative)
                                    .Select(x => new OrganizationUserVM
                                    {
                                        OrganizationUserGid = x.Gid,
                                        FullName = x.FullName,
                                        NotificateUser = false,
                                        UserType = x.UserTypeId
                                    });

            var orgUsers = repo.AllReadonly<UserOrganizationCase>()
                                    .Where(x => x.CaseId == caseModel.CaseId && x.OrganizationUserId == userContext.OrganizationUserId)
                                    .Select(x => new OrganizationUserVM
                                    {
                                        OrganizationUserGid = x.Gid,
                                        FullName = x.UserRegistration.FullName,
                                        NotificateUser = x.NotificateUser,
                                        UserType = x.UserRegistration.UserTypeId
                                    });

            return representatives.Union(orgUsers)
                        .OrderBy(x => x.UserType)
                        .ThenBy(x => x.FullName)
                        .AsQueryable();
        }

        public IQueryable<OrganizationUserListVM> SelectOrganizationUsersByOrganization()
        {
            if (userContext.UserType != NomenclatureConstants.UserTypes.OrganizationRepresentative)
            {
                return null;
            }

            return repo.AllReadonly<UserRegistration>()
                  .Where(x => x.OrganizationUserId == userContext.OrganizationUserId)
                  .Select(x => new OrganizationUserListVM
                  {
                      Gid = x.Gid,
                      FullName = x.FullName,
                      Email = x.Email,
                      NotificateUser = false,
                      UserType = x.UserTypeId,
                      IsComfirmed = x.ComfirmedDate != null,
                      IsActivated = x.IsComfirmedEGN,
                      IsActive = x.IsActive
                  }).OrderBy(x => x.UserType).ThenBy(x => x.FullName);

        }

        public async Task<List<SelectListItem>> GetDDL_OrganizationUsers(int? userType = null)
        {
            Expression<Func<UserRegistration, bool>> whereUserType = x => true;
            if (userType.HasValue)
            {
                whereUserType = x => x.UserTypeId == userType.Value;
            }
            return await repo.AllReadonly<UserRegistration>()
                                    .Where(x => x.OrganizationUserId == userContext.OrganizationUserId)
                                    .Where(whereUserType)
                                    .Select(x => new SelectListItem
                                    {
                                        Value = x.Gid.ToString(),
                                        Text = $"{x.FullName} {((x.UserTypeId == NomenclatureConstants.UserTypes.OrganizationRepresentative) ? "Представляващ" : "Юрист")} {((!x.IsActive) ? "Неактивен" : "")}"
                                    }).ToListAsync();
        }

        public async Task<SaveResultVM> ValidateOrganizationUser(UserRegistrationVM model)
        {
            model.Sanitize();
            var result = new SaveResultVM();
            if (string.IsNullOrEmpty(model.EGN))
            {
                result.AddError("Невалидно ЕГН.", nameof(UserRegistrationVM.EGN));
            }
            else
            {
                if (!Vaidations.IsValidPersonIdentifier(model.EGN))
                {
                    result.AddError("Невалидно ЕГН.", nameof(UserRegistrationVM.EGN));
                }

                if (await repo.AllReadonly<UserRegistration>()
                              .Where(x => x.OrganizationUserId == userContext.OrganizationUserId)
                          .Where(x => x.EGN == model.EGN)
                          .Where(x => x.Gid != (model.Gid ?? Guid.Empty))
                          .Where(x => x.DeniedDate == null)
                          .Where(x => x.IsActive == true)
                          .AnyAsync())
                {
                    result.AddError("Съществува юрист с това ЕГН.", nameof(UserRegistrationVM.EGN));
                }
            }
            if (string.IsNullOrEmpty(model.FullName))
            {
                result.AddError("Невалидно име.", nameof(UserRegistrationVM.FullName));
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                result.AddError("Невалидна електронна поща.", nameof(UserRegistrationVM.Email));
            }
            else
            {
                if (await repo.AllReadonly<UserRegistration>()
                                    .Where(x => x.OrganizationUserId == userContext.OrganizationUserId)
                                .Where(x => x.Email == model.Email)
                                .Where(x => x.Gid != (model.Gid ?? Guid.Empty))
                                .Where(x => x.DeniedDate == null)
                                .Where(x => x.IsActive == true)
                                .AnyAsync())
                {
                    result.AddError("Съществува юрист с тази електронна поща.", nameof(UserRegistrationVM.Email));
                }
            }
            return result;
        }

        public async Task<UserRegistrationVM> OrganizationUserGetByGid(Guid gid)
        {
            if (userContext.UserType != NomenclatureConstants.UserTypes.OrganizationRepresentative)
            {
                return null;
            }
            return await repo.AllReadonly<UserRegistration>()
                                .Where(x => x.Gid == gid)
                                .Select(x => new UserRegistrationVM
                                {
                                    Gid = x.Gid,
                                    UserType = x.UserTypeId,
                                    FullName = x.FullName,
                                    EGN = x.EGN,
                                    Email = x.Email,
                                    IsActive = x.IsActive
                                }).FirstOrDefaultAsync();
        }

        public async Task<SaveResultVM> OrganizationCase_SaveData(OrganizationUserVM model)
        {
            var result = new SaveResultVM(false);
            var userModel = await GetByGidAsync<UserRegistration>(model.UserGid);
            if (!model.CaseGid.HasValue)
            {
                result.AddError("Невалидно дело.");
                return result;
            }
            var caseModel = await GetByGidAsync<Case>(model.CaseGid.Value);
            if (userModel == null)
            {
                result.AddError("Невалиден юрист.", nameof(OrganizationUserVM.UserGid));
                return result;
            }
            if (caseModel == null)
            {
                result.AddError("Невалидно дело.");
                return result;
            }
            if (!(userContext.OrganizationUserId > 0))
            {
                result.AddError("Невалидна организация.");
                return result;
            }
            if (await repo.AllReadonly<UserOrganizationCase>()
                            .Where(x => x.UserRegistrationId == userModel.Id
                            && x.CaseId == caseModel.CaseId
                            && x.Gid != model.OrganizationUserGid)
                            .AnyAsync())
            {
                result.AddError("Избраният юрист вече е добавен по делото.", nameof(OrganizationUserVM.UserGid));
                return result;
            }
            if (model.OrganizationUserGid.HasValue)
            {
                var entity = await GetByGidAsync<UserOrganizationCase>(model.OrganizationUserGid.Value);
                if (entity == null)
                {
                    result.AddError("Невалиден идентификатор.");
                    return result;
                }
                entity.UserRegistrationId = userModel.Id;
                entity.NotificateUser = model.NotificateUser;
                entity.IsActive = model.IsActive;
                entity.UserWrtId = userContext.UserId;
                entity.DateWrt = DateTime.Now;
            }
            else
            {
                if (!await repo.AllReadonly<UserAssignment>()
                                .Where(x => x.CaseId == caseModel.CaseId && x.UserRegistrationId == userContext.OrganizationUserId && x.IsActive)
                                .AnyAsync())
                {
                    result.AddError("Нямате достъп до избраното дело.");
                    return result;
                }

                var entity = new UserOrganizationCase()
                {
                    CaseId = caseModel.CaseId,
                    Gid = Guid.NewGuid(),
                    DateWrt = DateTime.Now,
                    UserWrtId = userContext.UserId,
                    UserRegistrationId = userModel.Id,
                    OrganizationUserId = userContext.OrganizationUserId.Value,
                    IsActive = model.IsActive,
                    NotificateUser = model.NotificateUser,
                };
                await repo.AddAsync(entity);
            }

            try
            {
                await repo.SaveChangesAsync();
                result.Result = true;
                result.Message = SaveResultVM.MessageSaveOk;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return result;
        }
    }
}

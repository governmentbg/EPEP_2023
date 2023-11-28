using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Payment;
using Epep.Core.ViewModels.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class AdministrativeService : BaseService, IAdministrativeService
    {
        private readonly IUserContext userContext;
        private readonly IPaymentService paymentService;
        public AdministrativeService(
            IRepository _repo,
            IPaymentService paymentService,
            ILogger<AdministrativeService> _logger,
            IUserContext _userContext
            )
        {
            this.repo = _repo;
            this.logger = _logger;
            userContext = _userContext;
            this.paymentService = paymentService;
        }

        public IQueryable<UserRegistrationVM> NewOrganizationUsers()
        {
            var orgUsers = repo.AllReadonly<UserRegistration>()
                            .Where(x => x.UserTypeId == NomenclatureConstants.UserTypes.Organization)
                            .Where(x => x.ComfirmedDate == null && x.DeniedDate == null)
                            .OrderByDescending(x => x.CreateDate)
                            .Select(x => new UserRegistrationVM
                            {
                                Gid = x.Gid,
                                UserType = x.UserTypeId,
                                UserTypeName = x.UserRegistrationType.Name,
                                FullName = x.FullName,
                                UIC = x.UIC,
                                OrgFullName = ""
                            }).ToList();

            var orgRepUsers = repo.AllReadonly<UserRegistration>()
                            .Where(x => x.UserTypeId == NomenclatureConstants.UserTypes.OrganizationRepresentative)
                            .Where(x => x.ComfirmedDate == null && x.DeniedDate == null)
                            .Where(x => x.OrganizationUserId > 0)
                            .Where(x => x.OrganizationRegistration.ComfirmedDate != null)
                            .OrderByDescending(x => x.CreateDate)
                            .Select(x => new UserRegistrationVM
                            {
                                Gid = x.Gid,
                                UserType = x.UserTypeId,
                                UserTypeName = x.UserRegistrationType.Name,
                                FullName = x.FullName,
                                UIC = x.EGN,
                                OrgFullName = x.OrganizationRegistration.FullName
                            }).ToList();

            return orgUsers.Union(orgRepUsers).AsQueryable();
        }

        public IQueryable<UserRegistrationVM> SelectUsers(UserFilterVM filter)
        {
            filter.Sanitize();
            Expression<Func<UserRegistration, bool>> whereUserType = x => true;
            if (filter.UserType > 0)
            {
                whereUserType = x => x.UserTypeId == filter.UserType;
            }
            Expression<Func<UserRegistration, bool>> whereFullName = x => true;
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                whereFullName = x => EF.Functions.Like(x.FullName, filter.FullName.ToPaternSearch());
            }
            Expression<Func<UserRegistration, bool>> whereUIC = x => true;
            if (!string.IsNullOrEmpty(filter.UIC))
            {
                whereUIC = x => x.UIC == filter.UIC || x.EGN == filter.UIC;
            }
            Expression<Func<UserRegistration, bool>> whereEmail = x => true;
            if (!string.IsNullOrEmpty(filter.Email))
            {
                whereEmail = x => EF.Functions.Like(x.Email, filter.Email.ToPaternSearch());
            }
            Expression<Func<UserRegistration, bool>> whereActive = x => true;
            if (filter.ActiveOnly)
            {
                whereActive = x => x.IsActive;
            }

            Expression<Func<UserRegistration, bool>> whereComfirmed = x =>
            (NomenclatureConstants.UserTypes.OrganizationTypes.Contains(x.UserTypeId) && x.ComfirmedDate != null) || !NomenclatureConstants.UserTypes.OrganizationTypes.Contains(x.UserTypeId);
            if (filter.ComfirmedMode > 0)
                switch (filter.ComfirmedMode)
                {
                    case 1:
                        //Непотвърдени
                        whereComfirmed = x => NomenclatureConstants.UserTypes.OrganizationTypes.Contains(x.UserTypeId)
                         && x.ComfirmedDate == null && x.DeniedDate == null;
                        break;
                    case 2:
                        //Потвърдени
                        whereComfirmed = x => NomenclatureConstants.UserTypes.OrganizationTypes.Contains(x.UserTypeId)
                         && x.ComfirmedDate != null;
                        break;
                    case 3:
                        //Отказани
                        whereComfirmed = x => NomenclatureConstants.UserTypes.OrganizationTypes.Contains(x.UserTypeId)
                         && x.DeniedDate != null;
                        break;
                }

            return repo.AllReadonly<UserRegistration>()
                            .Where(whereUserType)
                            .Where(whereFullName)
                            .Where(whereUIC)
                            .Where(whereEmail)
                            .Where(whereActive)
                            .Where(whereComfirmed)
                            .Select(x => new UserRegistrationVM
                            {
                                Gid = x.Gid,
                                Email = x.Email,
                                UserType = x.UserTypeId,
                                UserTypeName = x.UserRegistrationType.Name,
                                FullName = x.FullName,
                                IsActive = x.IsActive,
                                UIC = (x.UserTypeId == NomenclatureConstants.UserTypes.Organization) ? x.UIC : x.EGN,
                                OrgFullName = (x.OrganizationUserId > 0) ? x.OrganizationRegistration.FullName : null,
                                CourtName = (x.CourtId > 0) ? x.Court.Name : null
                            });
        }

        public async Task<SaveResultVM> SaveUserComfirm(UserComfirmVM model)
        {
            var user = await GetByGidAsync<UserRegistration>(model.Gid);
            if (user == null)
            {
                return new SaveResultVM(false);
            }

            if (!model.Comfirm)
            {
                if (string.IsNullOrEmpty(model.DeniedDescription))
                {
                    var res = new SaveResultVM(false);
                    res.AddError("Полето е задължително.", nameof(UserComfirmVM.DeniedDescription));
                    return res;
                }
            }

            user.ComfirmedUserId = userContext.UserId;
            user.ModifyDate = DateTime.Now;
            string audiInfo = "";
            if (model.Comfirm)
            {
                user.ComfirmedDate = user.ModifyDate;

                audiInfo = $"Потвърден профил на {user.FullName}.";
            }
            else
            {
                user.DeniedDate = user.ModifyDate;
                user.DeniedDescription = model.DeniedDescription;
                user.IsActive = false;

                audiInfo = $"ОТКАЗАН профил на {user.FullName}. Основание: {model.DeniedDescription}";
            }
            try
            {
                await repo.SaveChangesAsync();
                var result = new SaveResultVM(true);
                result.AuditInfo = audiInfo;
                return result;
            }
            catch (DbUpdateException dbe)
            {
                return new SaveResultVM(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SaveUserComfirm");
                return new SaveResultVM(false);
            }
        }

        public IQueryable<LawyerVM> SelectLawyers(FilterLawyerVM filter)
        {
            filter.Sanitize();
            Expression<Func<Lawyer, bool>> whereName = x => true;
            if (!string.IsNullOrEmpty(filter.Name))
            {
                whereName = x => EF.Functions.Like(x.Name, filter.Name.ToPaternSearch());
            }
            Expression<Func<Lawyer, bool>> whereUIC = x => true;
            if (!string.IsNullOrEmpty(filter.Uic))
            {
                whereUIC = x => x.Uic == filter.Uic;
            }
            Expression<Func<Lawyer, bool>> whereNumber = x => true;
            if (!string.IsNullOrEmpty(filter.Number))
            {
                whereNumber = x => EF.Functions.Like(x.Number, filter.Number.ToPaternSearch());
            }
            Expression<Func<Lawyer, bool>> whereCollege = x => true;
            if (!string.IsNullOrEmpty(filter.College))
            {
                whereCollege = x => EF.Functions.Like(x.College, filter.College.ToPaternSearch());
            }



            return repo.AllReadonly<Lawyer>()
                            .Where(whereName)
                            .Where(whereNumber)
                            .Where(whereUIC)
                            .Where(whereCollege)
                            .OrderBy(x => x.Name)
                            .Select(x => new LawyerVM
                            {
                                Gid = x.Gid,
                                Name = x.Name,
                                LawyerTypeName = x.LawyerType.Name,
                                Number = x.Number,
                                College = x.College,
                                Uic = x.Uic
                            });
        }

        public async Task<SaveResultVM> Lawyer_SaveData(Lawyer model)
        {
            if (model == null)
            {
                return new SaveResultVM(false);
            }
            var result = new SaveResultVM(true);
            var auditInfo = "";
            if (model.LawyerId > 0)
            {

                if (await repo.AllReadonly<Lawyer>().Where(x => x.Number == model.Number && x.LawyerId != model.LawyerId).AnyAsync())
                {
                    result.AddError("Вече съществува адвокат с този номер.", nameof(Lawyer.Number));
                    return result;
                }

                //Update
                var saved = await repo.GetByIdAsync<Lawyer>(model.LawyerId);

                auditInfo = await getAudiInfo_Lawyer(model, saved);

                saved.LawyerTypeId = model.LawyerTypeId;
                saved.Number = model.Number;
                saved.Uic = model.Uic;
                saved.Name = model.Name;
                saved.College = model.College;
                saved.ModifyDate = DateTime.Now;
            }
            else
            {
                if (await repo.AllReadonly<Lawyer>().Where(x => x.Number == model.Number).AnyAsync())
                {
                    result.AddError("Вече съществува адвокат с този номер.", nameof(Lawyer.Number));
                    return result;
                }

                auditInfo = await getAudiInfo_Lawyer(model, null);
                model.Gid = Guid.NewGuid();
                model.CreateDate = DateTime.Now;
                model.ModifyDate = model.CreateDate;
                await repo.AddAsync(model);
            }
            await repo.SaveChangesAsync();
            result.AuditInfo = auditInfo;
            return result;
        }

        private async Task<string> getAudiInfo_Lawyer(Lawyer updated, Lawyer saved)
        {
            var lType = (await repo.GetByIdAsync<LawyerType>(updated.LawyerTypeId)).Name;

            string result = "";
            if (saved == null)
            {
                result += formatAuditRow("Вид", lType);
                result += formatAuditRow("Имена", updated.Name);
                result += formatAuditRow("Номер", updated.Number);
                result += formatAuditRow("Колегия", updated.College);
                result += formatAuditRow("Идентификатор", updated.Uic);
            }
            else
            {
                var slType = lType;
                if (saved.LawyerTypeId != updated.LawyerTypeId)
                {
                    slType = (await repo.GetByIdAsync<LawyerType>(saved.LawyerTypeId)).Name;
                    result += formatAuditRow("Вид", lType, slType, true);
                }
                if (updated.Name != saved.Name)
                {
                    result += formatAuditRow("Имена", updated.Name, saved.Name, true);
                }
                if (updated.Number != saved.Number)
                {
                    result += formatAuditRow("Номер", updated.Number, saved.Number, true);
                }
                if (updated.College != saved.College)
                {
                    result += formatAuditRow("Колегия", updated.College, saved.College, true);
                }
                if (updated.Uic != saved.Uic)
                {
                    result += formatAuditRow("Идентификатор", updated.Uic, saved.Uic, true);
                }
            }
            return result;


        }



        public async Task<ManageObligationResultVM> InitTestPayment(TestPaymentVM model, string requestSchema)
        {

            var court = await GetByIdAsync<Court>(model.CourtId);

            var obligation = new MoneyObligation()
            {
                Gid = Guid.NewGuid(),
                AttachmentType = NomenclatureConstants.AttachedTypes.SignTempFile,
                ParentId = 0,
                UserRegistrationId = userContext.UserId,
                ClientCode = court.Code,
                ParentDescription = $"Плащане към съд {court.Name}",
                Description = "Тестово плащане",
                MoneyObligationTypeId = NomenclatureConstants.MoneyObligationTypes.CourtTax,
                MoneyCurrencyId = NomenclatureConstants.Currencies.BGN,
                MoneyAmount = model.Amount,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
            };


            await repo.AddAsync(obligation);
            await repo.SaveChangesAsync();


            return await paymentService.ManageObligationPayments(obligation.Gid, requestSchema);
        }


    }
}

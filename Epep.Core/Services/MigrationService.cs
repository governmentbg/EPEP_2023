using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Epep.Core.Services
{
    public class MigrationService : BaseService, IMigrationService
    {
        private readonly UserManager<UserRegistration> userManager;
        public MigrationService(
            UserManager<UserRegistration> userManager,
            IRepository repo)
        {
            this.userManager = userManager;
            this.repo = repo;
        }

        public async Task<SaveResultVM> MigrateData()
        {
            var result = new SaveResultVM();

            //var personRegistrations = await repo.AllReadonly<PersonRegistration>()
            //                           .Include(x => x.User)
            //                           .Include(x => x.PersonAssignments)
            //                           .ThenInclude(x => x.Side)
            //                            .Where(x => x.Description == "migrate")
            //                           //.Where(x => x.User.IsActive)
            //                           .ToListAsync();

            var lawyerRegistrations = await repo.AllReadonly<LawyerRegistration>()
                                       .Include(x => x.User)
                                       .Include(x => x.Lawyer)
                                       //.Include(x => x.LawyerAssignments)
                                       //.ThenInclude(x => x.Side)
                                       .Where(x => x.Description == "migrate2903")
                                       .OrderByDescending(x => x.CreateDate)
                                       .ToListAsync();

            var lawyerAssignments = await repo.AllReadonly<LawyerAssignment>()
                                                .Include(x => x.Side)
                                                .Include(x => x.Lawyer)
                                                .ToListAsync();


            int savedPerson = 0;
            int savedLawyer = 0;

            int dbSaved = 0;
            //foreach (var person in personRegistrations)
            //{
            //    if (string.IsNullOrEmpty(person.EGN))
            //    {
            //        continue;
            //    }
            //    if (await migrationPerson(person))
            //    {
            //        savedPerson++;
            //    }

            //    dbSaved++;

            //    if (dbSaved > 50)
            //    {
            //        repo.ChangeTrackerClear();
            //        dbSaved = 0;
            //    }
            //}

            foreach (var lawyer in lawyerRegistrations.Where(x => !string.IsNullOrEmpty(x.Lawyer.Uic)))
            {
                if (await migrationLawyer(lawyer, lawyerAssignments))
                {
                    savedLawyer++;
                }

                dbSaved++;

                if (dbSaved > 50)
                {
                    repo.ChangeTrackerClear();
                    dbSaved = 0;
                }
            }

            //savedPerson =4747; savedLawyer = 5447
            //достъпи = 157368
            result.Message = $"Мигрирани физически лица: {savedPerson}; Мигрирани адвокати: {savedLawyer}";
            return result;
        }

        private async Task<bool> migrationPerson(PersonRegistration person)
        {
            var userModel = await repo.AllReadonly<UserRegistration>()
                                .Where(x => x.Gid == person.Gid)
                                .FirstOrDefaultAsync();
            var saved = false;

            List<UserAssignment> existingAssingments = new List<UserAssignment>();

            if (userModel == null)
            {
                var userReg = new UserRegistration()
                {
                    UserTypeId = NomenclatureConstants.UserTypes.Person,
                    UserName = Guid.NewGuid().ToString().ToLower().Replace("-", ""),
                    EGN = person.EGN,
                    FullName = person.Name,
                    Gid = person.Gid,
                    Email = person.Email,
                    EmailConfirmed = true,
                    CreateDate = person.CreateDate,
                    ModifyDate = person.ModifyDate,
                    IsActive = true
                };

                var saveUseRes = await userManager.CreateAsync(userReg);
                if (!saveUseRes.Succeeded)
                {
                    return false;
                }
                userModel = userReg;

            }
            else
            {
                existingAssingments = await repo.AllReadonly<UserAssignment>()
                                            .Where(x => x.UserRegistrationId == userModel.Id)
                                            .ToListAsync();
            }

            foreach (var assignment in person.PersonAssignments)
            {
                if (existingAssingments.Any(x => x.Gid == assignment.Gid))
                {
                    continue;
                }

                var newAssignment = new UserAssignment()
                {
                    Gid = assignment.Gid,
                    LegacyGid = person.Gid,
                    AssignmentRoleId = NomenclatureConstants.UserAssignmentRoles.Side,
                    Date = assignment.Date,
                    SideId = assignment.SideId ?? 0,
                    CaseId = assignment.Side?.CaseId ?? 0,
                    CreateDate = assignment.CreateDate,
                    ModifyDate = assignment.ModifyDate,
                    IsActive = assignment.IsActive ?? true,
                    UserRegistrationId = userModel.Id
                };

                if (newAssignment.SideId > 0 && newAssignment.CaseId > 0)
                {
                    await repo.AddAsync(newAssignment);
                    saved = true;
                }
            }
            if (saved)
            {
                await repo.SaveChangesAsync();
            }

            return true;
        }


        private async Task<bool> migrationLawyer(LawyerRegistration lawyerRegistration, List<LawyerAssignment> lawyerAssignments)
        {
            var userModel = await repo.AllReadonly<UserRegistration>()
                                .Where(x => x.Gid == lawyerRegistration.Gid)
                                .FirstOrDefaultAsync();

            if (userModel == null && !string.IsNullOrEmpty(lawyerRegistration.Lawyer.Uic))
            {
                userModel = await repo.AllReadonly<UserRegistration>()
                                .Where(x => x.EGN == lawyerRegistration.Lawyer.Uic && x.UserTypeId == NomenclatureConstants.UserTypes.Person)
                                .FirstOrDefaultAsync();
            }
            var saved = false;

            List<UserAssignment> existingAssingments = new List<UserAssignment>();

            if (userModel == null)
            {
                var userReg = new UserRegistration()
                {
                    UserTypeId = NomenclatureConstants.UserTypes.Person,
                    UserName = Guid.NewGuid().ToString().ToLower().Replace("-", ""),
                    EGN = lawyerRegistration.Lawyer.Uic,
                    FullName = lawyerRegistration.Lawyer.Name,
                    Gid = lawyerRegistration.Gid,
                    Email = lawyerRegistration.User.Username,
                    EmailConfirmed = true,
                    CreateDate = lawyerRegistration.CreateDate,
                    ModifyDate = lawyerRegistration.ModifyDate,
                    IsActive = true
                };

                var saveUseRes = await userManager.CreateAsync(userReg);
                if (!saveUseRes.Succeeded)
                {
                    return false;
                }
                userModel = userReg;

            }
            else
            {
                existingAssingments = await repo.AllReadonly<UserAssignment>()
                                            .Where(x => x.UserRegistrationId == userModel.Id)
                                            .ToListAsync();
            }

            foreach (var assignment in lawyerAssignments.Where(x => x.Lawyer.Uic == userModel.EGN))
            {
                if (existingAssingments.Any(x => x.Gid == assignment.Gid))
                {
                    continue;
                }

                var newAssignment = new UserAssignment()
                {
                    Gid = assignment.Gid,
                    LegacyGid = lawyerRegistration.Gid,
                    AssignmentRoleId = NomenclatureConstants.UserAssignmentRoles.Lawyer,
                    Date = assignment.Date,
                    SideId = assignment.SideId ?? 0,
                    CaseId = assignment.Side?.CaseId ?? 0,
                    CreateDate = assignment.CreateDate,
                    ModifyDate = assignment.ModifyDate,
                    IsActive = assignment.IsActive,
                    UserRegistrationId = userModel.Id
                };

                if (newAssignment.SideId > 0 && newAssignment.CaseId > 0)
                {
                    await repo.AddAsync(newAssignment);
                    saved = true;
                }
            }
            if (saved)
            {
                await repo.SaveChangesAsync();
            }

            return true;
        }
    }
}

using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.Core.Contracts
{
    public interface IUserService : IBaseService
    {
        Task<UserRegistration> FindByUIC(int loginUserType, string uic, string organizationUic = null);
        Task<string> GetAudiInfo_User(UserRegistration saved, UserRegistrationVM updated);
        Task<List<SelectListItem>> GetDDL_UserRegistrationTypes(bool adminsOnly = false);
        Task<List<SelectListItem>> GetDDL_UserVacationTypes();
        Task<UserAccessVM> ManageAccessForSide(Guid sideGid);
        Task<string> ManageAccessGetDescription(UserAccessVM model);
        Task<SaveResultVM> SaveRegistrationForm(byte[] pdfDoc, long userId);
        Task<UserComfirmedInfoVM> SelectConfirmInfo(Guid gid);
        Task<UserRegistrationVM> SelectDataForProfile(Guid? gid = null);
        Task<SaveResultVM> UserVacation_SaveData(UserVacation model);
        IQueryable<UserVacationVM> UserVacation_Select();
        Task<SaveResultVM> ValidateUserRegistration(UserRegistrationVM model);
    }
}

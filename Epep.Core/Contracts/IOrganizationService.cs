using Epep.Core.ViewModels;
using Epep.Core.ViewModels.Case;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.User;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.Core.Contracts
{
    public interface IOrganizationService : IBaseService
    {
        Task<List<SelectListItem>> GetDDL_OrganizationUsers(int? userType = null);
        Task<SaveResultVM> OrganizationCase_SaveData(OrganizationUserVM model);
        Task<UserRegistrationVM> OrganizationUserGetByGid(Guid gid);
        Task<IQueryable<OrganizationUserVM>> SelectOrganizationUsersByCase(GidLoaderVM loader);
        Task<OrganizationUserVM> SelectOrganizationUsersByGid(Guid gid);
        IQueryable<OrganizationUserListVM> SelectOrganizationUsersByOrganization();
        Task<SaveResultVM> ValidateOrganizationUser(UserRegistrationVM model);
    }
}

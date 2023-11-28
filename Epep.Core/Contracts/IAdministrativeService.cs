using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Payment;
using Epep.Core.ViewModels.User;

namespace Epep.Core.Contracts
{
    public interface IAdministrativeService : IBaseService
    {
        Task<ManageObligationResultVM> InitTestPayment(TestPaymentVM model, string requestSchema);
        Task<SaveResultVM> Lawyer_SaveData(Lawyer model);
        IQueryable<UserRegistrationVM> NewOrganizationUsers();
        Task<SaveResultVM> SaveUserComfirm(UserComfirmVM model);
        IQueryable<LawyerVM> SelectLawyers(FilterLawyerVM filter);
        IQueryable<UserRegistrationVM> SelectUsers(UserFilterVM filter);
    }
}

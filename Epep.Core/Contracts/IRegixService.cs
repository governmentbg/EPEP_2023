using Epep.Core.ViewModels.Regix;

namespace Epep.Core.Contracts
{
    public interface IRegixService
    {
        Task<EntityInfoVM> GetEntityInfo(string uic);
    }
}

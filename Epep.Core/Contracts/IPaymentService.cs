using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Payment;

namespace Epep.Core.Contracts
{
    public interface IPaymentService : IBaseService
    {
        Task<PaymentRegisterResultVM> CreatePaymentFromObligation(Guid obligationGid, string requestScheme);
        Task<ManageObligationResultVM> ManageObligationPayments(Guid obligationGid, string requestScheme);

        //Task<SaveResultVM> CheckExistingPaymentStatus(IEnumerable<PaymentVM> payments);
        //Task<PaymentRegisterResultVM> CreatePaymentFromDocument(long electronicDocumentId, string requestScheme);
        IQueryable<ObligationVM> ObligationSelect(int type, long parentId);
        IQueryable<ObligationVM> Select(FilterObligationVM filter);

        //IQueryable<PaymentVM> Select(int type, long parentId);
        Task<SaveResultVM> UpdateStatus(Guid paymentGid, bool isSuccess);
    }
}

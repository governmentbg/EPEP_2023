using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Common;
using Epep.Core.ViewModels.Payment;
using IO.PaymentProvider.Models;
using IO.RegixClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly IUrlHelper urlHelper;
        private readonly IPaymentProviderClient paymentClient;
        private readonly IUserContext userContext;
        public PaymentService(
            IPaymentProviderClient paymentClient,
            IUserContext userContext,
            IUrlHelper urlHelper,
            IRepository repo)
        {
            this.repo = repo;
            this.userContext = userContext;
            this.paymentClient = paymentClient;
            this.urlHelper = urlHelper;
        }

        public IQueryable<ObligationVM> Select(FilterObligationVM filter)
        {
            filter.Sanitize();
            Expression<Func<MoneyObligation, bool>> whereUser = x => x.UserRegistrationId == userContext.UserId;
            if (NomenclatureConstants.UserTypes.OrganizationTypes.Contains(userContext.UserType))
            {
                whereUser = x => x.UserRegistrationId == userContext.OrganizationUserId;
            }



            Expression<Func<MoneyObligation, bool>> whereDateFrom = x => true;
            if (filter.DateFrom != null)
            {
                whereDateFrom = x => x.CreateDate <= filter.DateFrom;
            }
            Expression<Func<MoneyObligation, bool>> whereDateTo = x => true;
            if (filter.DateTo != null)
            {
                whereDateTo = x => x.CreateDate <= filter.DateTo.MakeEndDate();
            }
            Expression<Func<MoneyObligation, bool>> whereState = x => true;
            switch (filter.PaymentStatus)
            {
                case 1:
                    whereState = x => x.PaymentDate != null;
                    break;
                case 2:
                    whereState = x => x.PaymentDate == null;
                    break;
            }
            Expression<Func<MoneyObligation, bool>> whereParent = x => true;
            if (!string.IsNullOrEmpty(filter.Description))
            {
                whereParent = x => EF.Functions.Like(x.ParentDescription, filter.Description.ToPaternSearch());
            }

            return repo.AllReadonly<MoneyObligation>()
                            .Where(whereUser)
                            .Where(whereDateFrom)
                            .Where(whereDateTo)
                            .Where(whereState)
                            .Where(whereParent)
                            .OrderByDescending(x => x.Id)
                            .Select(x => new ObligationVM
                            {
                                Gid = x.Gid,
                                ObligationTypeName = x.ObligationType.Name,
                                CurrencyName = x.Currency.Code,
                                ParentDescription = x.ParentDescription,
                                CreateDate = x.CreateDate,
                                Description = x.Description,
                                MoneyAmount = x.MoneyAmount,
                                PaymentDate = x.PaymentDate
                            });
        }

        public IQueryable<ObligationVM> ObligationSelect(int type, long parentId)
        {
            return repo.AllReadonly<MoneyObligation>()
                            .Where(x => x.AttachmentType == type && x.ParentId == parentId)
                            .OrderByDescending(x => x.Id)
                            .Select(x => new ObligationVM
                            {
                                Gid = x.Gid,
                                ObligationTypeName = x.ObligationType.Name,
                                CurrencyName = x.Currency.Code,
                                ParentDescription = x.ParentDescription,
                                CreateDate = x.CreateDate,
                                Description = x.Description,
                                MoneyAmount = x.MoneyAmount,
                                PaymentDate = x.PaymentDate
                            });
        }

        //public IQueryable<PaymentVM> Select(int type, long parentId)
        //{
        //    return repo.AllReadonly<ElectronicPayment>()
        //                    .Where(x => x.AttachmentType == type && x.ParentId == parentId)
        //                    .OrderByDescending(x => x.Id)
        //                    .Select(x => new PaymentVM
        //                    {
        //                        Id = x.Id,
        //                        Gid = x.Gid,
        //                        PaymentCode = x.PaymentKey,
        //                        ParentDescription = x.ParentDescription,
        //                        Description = x.Description,
        //                        MoneyAmount = x.MoneyAmount,
        //                        MoneyCurrency = x.MoneyCurrency.Code,
        //                        IsPaid = x.PaymentDate != null,
        //                        IsFailed = x.FailDate != null
        //                    });
        //}

        public async Task<ManageObligationResultVM> ManageObligationPayments(Guid obligationGid, string requestScheme)
        {
            var result = new ManageObligationResultVM();
            var existingPayments = await repo.AllReadonly<ElectronicPayment>()
                                                .Where(x => x.Obligation.Gid == obligationGid)
                                                .Where(x => x.FailDate == null)
                                                .Select(x => new PaymentVM
                                                {
                                                    Id = x.Id,
                                                    Gid = x.Gid,
                                                    PaymentCode = x.PaymentKey,
                                                    MoneyAmount = x.MoneyAmount,
                                                    MoneyCurrency = x.MoneyCurrency.Code,
                                                    IsPaid = x.PaymentDate != null
                                                }).ToListAsync();

            if (existingPayments.Any())
            {
                foreach (var payment in existingPayments)
                {
                    var checkResult = await checkPaymentStatus(payment);
                    if (checkResult.Result)
                    {
                        result.Result = true;
                        result.PaymentChanged = true;
                        return result;
                    }
                    else
                    {
                        result.Message = checkResult.Message;
                    }
                }
            }
            else
            {
                var registerPaymentResult = await CreatePaymentFromObligation(obligationGid, requestScheme);
                result.Result = registerPaymentResult.Result;
                result.Message = registerPaymentResult.Message;
                result.CardFormUrl = registerPaymentResult.CardFormUrl;
            }
            return result;
        }

        public async Task<PaymentRegisterResultVM> CreatePaymentFromObligation(Guid obligationGid, string requestScheme)
        {
            var result = new PaymentRegisterResultVM();
            result.Result = false;

            var obligationInfo = await repo.AllReadonly<MoneyObligation>()
                                        .Where(x => x.Gid == obligationGid)
                                        .Select(x => new
                                        {
                                            x.Id,
                                            InvoiceNumber = x.Description,
                                            ClientCode = x.ClientCode,
                                            x.AttachmentType,
                                            ObligationCode = x.ObligationType.Code,
                                            Amount = x.MoneyAmount,
                                            CurrencyId = x.MoneyCurrencyId,
                                            CurrencyCode = x.Currency.Code,
                                        }).FirstOrDefaultAsync();

            if (obligationInfo == null)
            {
                result.Message = NomenclatureConstants.Messages.NotFound;
                return result;
            }

            var paymentGid = Guid.NewGuid();

            var paymentRegistrationModel = new PaymentRegistrationModel()
            {
                ClientCode = $"{obligationInfo.ClientCode}-{obligationInfo.ObligationCode}",
                Amount = (long)Math.Round(obligationInfo.Amount * 100M),
                Currency = obligationInfo.CurrencyCode,
                InvoiceNumber = paymentGid.ToString(),
                Language = "bg",
                SuccessUrl = urlHelper.Action("PaymentSuccess", "Document", new { payment = paymentGid }, requestScheme),
                FailUrl = urlHelper.Action("PaymentFailed", "Document", new { payment = paymentGid }, requestScheme)
            };

            if (obligationInfo.AttachmentType == NomenclatureConstants.AttachedTypes.SignTempFile)
            {
                paymentRegistrationModel.SuccessUrl = urlHelper.Action("TestPayment", "Admin", new { message = "OK" }, requestScheme);
                paymentRegistrationModel.FailUrl = urlHelper.Action("TestPayment", "Admin", new { message = "FAILED" }, requestScheme);
            }

            var apiResult = await paymentClient.Register(paymentRegistrationModel);
            if (apiResult != null)
            {

                if (!string.IsNullOrEmpty(apiResult.CardFormUrl))
                {
                    result.Result = true;
                    result.CardFormUrl = apiResult.CardFormUrl;
                    var newPayment = new ElectronicPayment()
                    {
                        Gid = paymentGid,
                        MoneyObligationId = obligationInfo.Id,
                        PaymentKey = apiResult.Gid,
                        MoneyAmount = obligationInfo.Amount,
                        MoneyCurrencyId = obligationInfo.CurrencyId,
                        CreateDate = DateTime.Now,
                        InvoiceNumber = obligationInfo.InvoiceNumber
                    };
                    await repo.AddAsync(newPayment);
                    await repo.SaveChangesAsync();
                }
                else
                {
                    result.Message = apiResult.ErrorMessage;
                }
            }

            return result;
        }

        public async Task<SaveResultVM> checkPaymentStatus(PaymentVM payment)
        {

            var checkResult = await paymentClient.Status(new PaymentStatusModel() { Gid = payment.PaymentCode });
            switch (checkResult.StatusCode)
            {
                case PaymentProviderConstants.Status.Paid:
                    return await UpdateStatus(payment.Gid, true);
                case PaymentProviderConstants.Status.Failed:
                    return await UpdateStatus(payment.Gid, false);
            }


            return new SaveResultVM(false);
        }

        public async Task<SaveResultVM> UpdateStatus(Guid paymentGid, bool isSuccess)
        {
            var payment = await GetByGidAsync<ElectronicPayment>(paymentGid);
            if (payment == null)
            {
                return new SaveResultVM(false, NomenclatureConstants.Messages.NotFound);
            }

            if (payment.FailDate != null || payment.PaymentDate != null)
            {
                return new SaveResultVM(false, "Транзакцията вече е обработена.");
            }
            var result = new SaveResultVM(false);

            var moneyObligation = await repo.GetByIdAsync<MoneyObligation>(payment.MoneyObligationId);

            ElectronicDocument electronicDocument = null;
            if (moneyObligation.AttachmentType == NomenclatureConstants.AttachedTypes.ElectronicDocument)
            {
                electronicDocument = await repo.GetByIdAsync<ElectronicDocument>(moneyObligation.ParentId);
                if (electronicDocument == null)
                {
                    return new SaveResultVM(false, NomenclatureConstants.Messages.NotFound);
                }
                result.ParentId = electronicDocument.Gid;
            }

            if (isSuccess)
            {
                //Допълнителна проверка дали плащането действително е отразено в PaymentProvider-а
                var checkResult = await paymentClient.Status(new PaymentStatusModel() { Gid = payment.PaymentKey });
                if (checkResult.StatusCode == PaymentProviderConstants.Status.Registered)
                {
                    result.Message = "Транзакцията още не е отразена.";
                    return result;
                }
                if (checkResult.StatusCode == PaymentProviderConstants.Status.Failed)
                {
                    payment.FailDate = DateTime.Now;
                    await repo.SaveChangesAsync();
                    result.Message = "Транзакцията е отказана.";
                    return result;
                }

                payment.PaymentDate = DateTime.Now;

                moneyObligation.PaymentDate = payment.PaymentDate;

                if (electronicDocument != null)
                {
                    electronicDocument.DatePaid = payment.PaymentDate;
                }


            }
            else
            {
                payment.FailDate = DateTime.Now;
            }

            await repo.SaveChangesAsync();
            result.Result = true;
            return result;
        }
    }
}

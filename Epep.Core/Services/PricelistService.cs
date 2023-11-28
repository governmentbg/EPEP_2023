using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Extensions;
using Epep.Core.Models;
using Epep.Core.ViewModels.Admin;
using Epep.Core.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class PricelistService : BaseService, IPricelistService
    {
        public PricelistService(
            IRepository _repo,
            ILogger<PricelistService> _logger)
        {
            this.repo = _repo;
            this.logger = _logger;
        }

        public IQueryable<PricelistVM> PricelistSelect(FilterPricelistVM filter)
        {
            filter.Sanitize();
            Expression<Func<MoneyPricelist, bool>> whereName = x => true;
            if (!string.IsNullOrEmpty(filter.Name))
            {
                whereName = x => EF.Functions.Like(x.Name, filter.Name.ToPaternSearch());
            }
            Expression<Func<MoneyPricelist, bool>> whereActive = x => true;
            if (filter.ActiveOnly)
            {
                whereActive = x => (x.DateTo ?? dtNow) >= dtNow.Date;
            }

            return repo.AllReadonly<MoneyPricelist>()
                            .Where(whereName)
                            .Where(whereActive)
                            .OrderByDescending(x => (x.DateTo))
                            .ThenBy(x => x.Name)
                            .Select(x => new PricelistVM
                            {
                                Id = x.Id,
                                Name = x.Name,
                                ShortName = x.ShortName,
                                IsActive = x.DateTo == null || x.DateTo > dtNow,
                                DocumentsList = x.PricelistDocuments.Select(d => d.ElectronicDocumentType.Name).ToArray()
                            });

        }

        public async Task<MoneyPricelist> PricelistGetById(long id)
        {
            var result = await repo.GetByIdAsync<MoneyPricelist>(id);
            if (result != null)
            {
                result.DocumentsIds = string.Join(',', (await repo.AllReadonly<MoneyPricelistDocument>()
                                                .Where(x => x.MoneyPricelistId == id)
                                                .Select(x => x.ElectronicDocumentTypeId.ToString())
                                                .ToArrayAsync()));
            }
            return result;
        }

        public async Task<SaveResultVM> PricelistSaveData(MoneyPricelist model)
        {
            long[] modelDocs = model.DocumentsIds.ToLongArray();
            if (model.Id > 0)
            {
                var saved = await repo.GetByIdAsync<MoneyPricelist>(model.Id);
                saved.Name = model.Name;
                saved.ShortName = model.ShortName;
                saved.Description = model.Description;
                saved.Code = model.Code;
                saved.DateFrom = model.DateFrom;
                saved.DateTo = model.DateTo;


                var savedDocs = await repo.All<MoneyPricelistDocument>()
                                            .Where(x => x.MoneyPricelistId == model.Id)
                                            .ToListAsync();
                repo.DeleteRange(savedDocs.Where(x => !modelDocs.Any(d => x.ElectronicDocumentTypeId == d)));
                var newDocs = modelDocs.Where(d => !savedDocs.Any(x => x.ElectronicDocumentTypeId == d)).Select(x => new MoneyPricelistDocument { MoneyPricelistId = model.Id, ElectronicDocumentTypeId = x, DateFrom = DateTime.Now.Date }).ToList();
                await repo.AddRangeAsync(newDocs);

            }
            else
            {
                model.PricelistDocuments = modelDocs.Select(x => new MoneyPricelistDocument { ElectronicDocumentTypeId = x, DateFrom = DateTime.Now.Date }).ToList();
                await repo.AddAsync(model);
            }

            await repo.SaveChangesAsync();
            return new SaveResultVM(true);
        }

        public IQueryable<PricelistValueVM> SelectPricelistValueByPricelist(long pricelistId)
        {
            return repo.AllReadonly<MoneyPricelistValue>()
                        .Where(x => x.MoneyPricelistId == pricelistId)
                        .OrderByDescending(x => x.DateFrom)
                        .Select(x => new PricelistValueVM
                        {
                            Id = x.Id,
                            Type = x.Type,
                            CurrencyCode = x.Currency.Code,
                            Value = x.Value,
                            MinValue = x.MinValue,
                            Procent = x.Procent,
                            DateFrom = x.DateFrom,
                            DateTo = x.DateTo
                        });
        }

        public async Task<SaveResultVM> PricelistValueSaveData(MoneyPricelistValue model)
        {
            var result = new SaveResultVM();
            switch (model.Type)
            {
                case NomenclatureConstants.MoneyValueTypes.Value:
                    model.Procent = null;
                    model.MinValue = null;
                    break;
                case NomenclatureConstants.MoneyValueTypes.Procent:
                    model.Value = null;
                    break;
                default:
                    result.AddError("Невалиден Тип стойност", nameof(model.Type));
                    return result;
            }
            if (model.Id > 0)
            {
                var saved = await repo.GetByIdAsync<MoneyPricelistValue>(model.Id);
                saved.MoneyCurrencyId = model.MoneyCurrencyId;
                saved.Type = model.Type;
                saved.Value = model.Value;
                saved.Procent = model.Procent;
                saved.MinValue = model.MinValue;
                saved.DateFrom = model.DateFrom;
                saved.DateTo = model.DateTo;
            }
            else
            {
                await repo.AddAsync(model);
            }

            try
            {
                await repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.Result = false;
            }
            return result;
        }


        public async Task<List<SelectListItem>> SelectPricelistsByDocument(long documentTypeId)
        {
            try
            {
                return await repo.AllReadonly<MoneyPricelistDocument>()
                                                .Where(x => x.ElectronicDocumentTypeId == documentTypeId)
                                                .Where(x => x.DateFrom <= dtNow && (x.DateTo ?? dtTomorow) > dtNow)
                                                .Select(x => new SelectListItem
                                                {
                                                    Value = x.MoneyPricelistId.ToString(),
                                                    Text = x.Pricelist.ShortName ?? x.Pricelist.Name
                                                }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<decimal> GetPrice(long pricelistId, decimal baseMoney = 0M, long currencyId = 1)
        {
            var priceValueRow = await repo.AllReadonly<MoneyPricelistValue>()
                                            .Where(x => x.MoneyPricelistId == pricelistId)
                                            .Where(x => x.MoneyCurrencyId == currencyId)
                                            .Where(x => x.DateFrom <= dtNow && (x.DateTo ?? dtTomorow) > dtNow)
                                            .FirstOrDefaultAsync();

            if (priceValueRow == null)
            {
                return -1M;
            }

            switch (priceValueRow.Type)
            {
                case NomenclatureConstants.MoneyValueTypes.Value:
                    return priceValueRow.Value ?? 0M;
                case NomenclatureConstants.MoneyValueTypes.Procent:
                    decimal result = (priceValueRow.Procent * baseMoney / (decimal)100M) ?? 0M;
                    if (result < priceValueRow.MinValue)
                    {
                        result = priceValueRow.MinValue ?? 0M;
                    }
                    return result;
                default:
                    return -1M;
            }
        }

        public async Task<List<SelectListItem>> GetDDL_Currencies()
        {
            try
            {
                return await repo.AllReadonly<MoneyCurrency>()
                                                .Select(x => new SelectListItem
                                                {
                                                    Value = x.Id.ToString(),
                                                    Text = x.Name
                                                }).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

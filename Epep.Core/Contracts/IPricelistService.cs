using Epep.Core.Models;
using Epep.Core.ViewModels.Admin;
using Epep.Core.ViewModels.Common;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epep.Core.Contracts
{
    public interface IPricelistService : IBaseService
    {
        Task<List<SelectListItem>> GetDDL_Currencies();
        Task<decimal> GetPrice(long pricelistId, decimal baseMoney = 0, long currencyId = 1);
        Task<MoneyPricelist> PricelistGetById(long id);
        Task<SaveResultVM> PricelistSaveData(MoneyPricelist model);
        IQueryable<PricelistVM> PricelistSelect(FilterPricelistVM filter);
        Task<SaveResultVM> PricelistValueSaveData(MoneyPricelistValue model);
        Task<List<SelectListItem>> SelectPricelistsByDocument(long documentTypeId);
        IQueryable<PricelistValueVM> SelectPricelistValueByPricelist(long pricelistId);
    }
}

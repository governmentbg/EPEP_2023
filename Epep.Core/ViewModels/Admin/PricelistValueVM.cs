using Epep.Core.Constants;
using Epep.Core.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Epep.Core.ViewModels.Admin
{
    public class PricelistValueVM
    {
        public long Id { get; set; }
        public long PricelistId { get; set; }
        public int Type { get; set; }
        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case NomenclatureConstants.MoneyValueTypes.Procent:
                        return "Процент";
                    default:
                        return "Стойност";
                }
            }
        }

        public string CurrencyCode { get; set; }

        public decimal? Value { get; set; }
        public decimal? Procent { get; set; }
        public decimal? MinValue { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public bool IsActive
        {
            get
            {
                return DateTo == null || DateTo > DateTime.Now;
            }
        }
    }
}

public class FilterPricelistVM
{
    [Display(Name = "Наименование")]
    public string Name { get; set; }

    [Display(Name = "Само активни тарифи")]
    public bool ActiveOnly { get; set; }

    public FilterPricelistVM()
    {
        ActiveOnly = true;
    }


    public void Sanitize()
    {
        Name = Name.EmptyToNull();
    }
}


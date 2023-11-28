using Epep.Core.ViewModels.Case;

namespace Epep.MobileApi.Models
{
    public class CaseListVM
    {
        public IEnumerable<CaseApiVM> Items { get; set; }
        public string? NextPageUrl { get; set; }
    }
}

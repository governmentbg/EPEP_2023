using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epep.MobileApi.Models
{
    public class NomenclaturesVM
    {
        public List<NomenclatureItemVM> Courts { get; set; }
        public List<NomenclatureItemVM> CaseKinds { get; set; }
        public List<NomenclatureItemVM> ActKinds { get; set; }
        public List<NomenclatureItemVM> Years { get; set; }
    }
}

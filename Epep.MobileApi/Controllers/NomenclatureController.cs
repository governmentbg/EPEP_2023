using Epep.Core.Contracts;
using Epep.MobileApi.Extensions;
using Epep.MobileApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Epep.MobileApi.Controllers
{
    [Produces("application/json")]
    public class NomenclatureController : BaseApiController
    {


        private readonly ILogger<NomenclatureController> _logger;
        private readonly INomenclatureService nomService;

        public NomenclatureController(ILogger<NomenclatureController> logger, INomenclatureService _nomService)
        {
            _logger = logger;
            nomService = _nomService;
        }

        [HttpGet(Name = "GetNomenclatures")]
        public async Task<NomenclaturesVM> GetNomenclatures()
        {
            return new NomenclaturesVM()
            {
                Courts = (await nomService.GetDDL_Courts()).ToSimpleNomenclature(),
                CaseKinds = (await nomService.GetDDL_CaseKind()).ToSimpleNomenclature(),
                ActKinds = (await nomService.GetDDL_ActKinds()).ToSimpleNomenclature(),
                Years = nomService.GetDDL_CaseYears().ToSimpleNomenclature()
            };
        }
    }
}
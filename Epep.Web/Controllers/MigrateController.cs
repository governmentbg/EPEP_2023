using Epep.Core.Contracts;
using Epep.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Epep.Web.Controllers
{
    [Authorize(Policy = GlobalAdminPolicyRequirement.Name)]
    public class MigrateController : Controller
    {
        private readonly IMigrationService migrationService;
        public MigrateController(
            IMigrationService migrationService
        )
        {
            this.migrationService = migrationService;
        }
        public async Task<IActionResult> Users()
        {
            try
            {
                
                var result = await migrationService.MigrateData();
                return Content($"Result:{result.Result};{result.Message}");
            }
            catch (Exception ex)
            {
                return Content($"{ex.Message}; inner:{ex.InnerException?.Message}");
            }
        }
    }
}

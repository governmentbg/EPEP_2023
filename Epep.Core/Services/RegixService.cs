using Epep.Core.Contracts;
using Epep.Core.Extensions;
using Epep.Core.Services.ViewModels.Regix;
using Epep.Core.ViewModels.Regix;
using IO.RegixClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Regix;

namespace Epep.Core.Services
{
    public class RegixService : IRegixService
    {
        private readonly IConfiguration config;
        private readonly ILogger<RegixService> logger;
        private readonly IServiceProvider serviceProvider;
        public RegixService(
            IServiceProvider serviceProvider,
            IConfiguration config,
            ILogger<RegixService> logger
            )
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.config = config;
        }

        private CallContext RegixCallContex()
        {
            return new CallContext()
            {
                AdministrationName = "Висш съдебен съвет",
                AdministrationOId = "2.16.100.1.1.511.1.3",
                EmployeeIdentifier = "1",
                EmployeeNames = "Администратор",
                EmployeePosition = "Администратор",
                LawReason = "Чл. 2, ал. 1 от ЗЕУ,  чл. 50, ал. 5 ГПК и чл. 52 ГПК",
                Remark = "",
                ServiceType = "За административна услуга",
                ServiceURI = "2.16.100.1.1.511.1.3"
            };
        }

        public async Task<EntityInfoVM> GetEntityInfo(string uic)
        {
            var organization = new EntityInfoVM()
            {
                Uic = uic
            };

            try
            {
                //var cert = new X509Certificate2("regix/vss-epep-regix.p12", "123456");
                //var newCert = new X509Certificate2(config.GetValue<string>("Regix:Certificate"), config.GetValue<string>("Regix:Password"));


                using (var scope = serviceProvider.CreateScope())
                {
                    var client = scope.ServiceProvider.GetService<IRegixClient>();

                    var trData = await client.TR_GetActualStateV3Async(uic, RegixCallContex());
                    if (trData.Deed != null)
                    {
                        var mappedTRdata = new IO.RegixClient.Models.RegixData.RegixActualStateV3ResponseVM();
                        var actualMaps = FileHelper.LoadDataFromFile<RegixMapActualStateModel>("regix/regix_map_actual_state.json")
                            .Select(x => (IO.RegixClient.Contracts.IRegixMapTRActualState)x).ToList();
                        var mapResult = client.TR_MapActualStateV3(trData, mappedTRdata, actualMaps);
                        mappedTRdata.ToOrganization(organization);
                    }
                    else
                    {
                        var bulstatData = await client.Bulstat_GetStateOfPlayMapped(uic, RegixCallContex());
                        bulstatData.ToOrganization(organization);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetEntityInfo UIC:{uic}");
            }
            return organization;
        }
    }
}

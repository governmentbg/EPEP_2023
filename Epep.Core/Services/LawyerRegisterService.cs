using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Integration.LawyerRegister;
using Epep.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Epep.Core.Services
{
    public class LawyerRegisterService : ILawyerRegisterService
    {
        public static string HttpClientName = "lawyerRegisterHttpClient";

        private readonly IRepository repo;
        private readonly HttpClient client;
        private string BaseUrl;
        private readonly INomenclatureService nomService;
        private ILogger<LawyerRegisterService> logger;

        public LawyerRegisterService(
            IHttpClientFactory _clientFactory,
            INomenclatureService _nomService,
            ILogger<LawyerRegisterService> _logger,
            IRepository _repo,
            IConfiguration config)
        {
            client = _clientFactory.CreateClient(HttpClientName);
            BaseUrl = config.GetValue<string>("LawyerRegister:URI");
            nomService = _nomService;
            repo = _repo;
            logger = _logger;
        }


        public async Task FetchLawyers()
        {
            LawyerDto[] lawyers = null;
            try
            {
                lawyers = await client.GetLawyers(this.BaseUrl);
                logger.LogError($"FetchLawyers, count fetched: {lawyers.Length}");
            }
            catch (Exception ex)
            {
                logger.LogError($"FetchLawyers, Error : {ex.Message}");
            }
            if (lawyers == null)
            {
                return;
            }
            var codes = await repo.AllReadonly<CodeMapping>().ToListAsync();
            var dtNow = DateTime.Now;
            int processedCount = 0;
            int totalProcessedCount = 0;
            int errorCount = 0;

            foreach (var lawyer in lawyers)
            {
                if (string.IsNullOrWhiteSpace(lawyer.identityNumber) || string.IsNullOrWhiteSpace(lawyer.name))
                {
                    continue;
                }

                totalProcessedCount++;
                processedCount++;
                if (processedCount > 1000)
                {
                    repo.ChangeTrackerClear();
                    processedCount = 0;
                    logger.LogError($"FetchLawyers, total processed: {totalProcessedCount}");
                }


                var model = mapToLawyer(lawyer, codes);
                if (model == null)
                {
                    errorCount++;
                    continue;
                }

                //continue;

                var saved = await repo.All<Lawyer>()
                                    .Where(x => x.Number == model.Number)
                                    .FirstOrDefaultAsync();

                if (saved != null)
                {
                    if (lawyerData(saved) != lawyerData(model))
                    {
                        saved.Name = model.Name;
                        saved.Uic = model.Uic;
                        saved.College = model.College;
                        saved.LawyerTypeId = model.LawyerTypeId;
                        saved.LawyerStateId = model.LawyerStateId;
                        saved.ModifyDate = DateTime.Now;
                        await repo.SaveChangesAsync();
                    }
                }
                else
                {
                    model.Gid = Guid.NewGuid();
                    model.CreateDate = DateTime.Now;
                    model.ModifyDate = DateTime.Now;
                    await repo.AddAsync(model);
                    await repo.SaveChangesAsync();
                }

            }

            logger.LogError($"FetchLawyers, Error count: {errorCount}");
            logger.LogError($"FetchLawyers, total processed: {totalProcessedCount}");
        }

        string lawyerData(Lawyer lawyer)
        {
            return $"{lawyer.Name}-{lawyer.Uic}-{lawyer.College}-{lawyer.LawyerTypeId}-{lawyer.LawyerStateId}";
        }


        Lawyer mapToLawyer(LawyerDto entity, List<CodeMapping> mapList)
        {
            if (entity == null)
            {
                return null;
            }
            entity.identityNumber = entity.identityNumber.Trim();
            entity.name = entity.name.Trim();
            var lawyerType = nomService.GetInnerCodeFromList(mapList, NomenclatureConstants.CodeMappingAlias.LawyerTypes, entity.type);
            if (string.IsNullOrEmpty(lawyerType))
            {
                return null;
            }
            var epepLawyerType = long.Parse(lawyerType);
            if (!NomenclatureConstants.LawyerTypes.LawyerLoginTypes.Contains(epepLawyerType))
            {
                return null;
            }

            var lawyerState = nomService.GetInnerCodeFromList(mapList, NomenclatureConstants.CodeMappingAlias.LawyerStates, entity.status);
            if (string.IsNullOrEmpty(lawyerState))
            {
                return null;
            }
            var epepLawyerState = long.Parse(lawyerState);
            if (!NomenclatureConstants.LawyerStates.AcceptedStates.Contains(epepLawyerState))
            {
                return null;
            }

            var result = new Lawyer()
            {
                Number = entity.identityNumber,
                LawyerTypeId = epepLawyerType,
                LawyerStateId = epepLawyerState,
                Name = entity.name,
                Uic = entity.egnOrBirthDate,
                College = nomService.GetInnerCodeFromList(mapList, NomenclatureConstants.CodeMappingAlias.LawyerCollages, entity.barAssociation),
            };

            return result;
        }

        /*
      public async Task FetchLawyersToBar()
      {
          LawyerDto[] lawyers = null;
          try
          {
              lawyers = await client.GetLawyers(this.BaseUrl);
          }
          catch (Exception ex)
          {

          }
          if (lawyers == null)
          {
              return;
          }
          var codes = await repo.AllReadonly<CodeMapping>().ToListAsync();
          int added = 0;
          var dtNow = DateTime.Now;
          foreach (var dto in lawyers)
          {

              var lb = new LawyerBar()
              {
                  Name = dto.name,
                  Number = dto.identityNumber,
                  Uic = dto.egnOrBirthDate,
                  College = dto.barAssociation,
                  LawyerStateName = dto.status,
                  LawyerTypeName = dto.type,
                  DateWrt = dtNow,
              };

              //var l = mapToLawyer(dto, codes);

              //if (l != null)
              //{
              //    lb.LawyerTypeId = l.LawyerTypeId;
              //    lb.LawyerStateId = l.LawyerStateId;
              //}

              var college = nomService.GetInnerCodeFromList(codes, NomenclatureConstants.CodeMappingAlias.LawyerCollages, dto.barAssociation);
              if (!string.IsNullOrEmpty(college))
              {
                  lb.College = college;
              }

              await repo.AddAsync(lb);
              added++;
              if (added > 100)
              {
                  await repo.SaveChangesAsync();
                  repo.ChangeTrackerClear();
                  added = 0;
              }
          }
          if (added > 0)
          {
              await repo.SaveChangesAsync();
          }

          return;
          foreach (var lawyer in lawyers)
          {
              var model = mapToLawyer(lawyer, null);
              if (model == null)
              {
                  continue;
              }

              //continue;

              var saved = await repo.All<Lawyer>()
                                  .Where(x => x.Number == model.Number)
                                  .FirstOrDefaultAsync();

              if (saved != null)
              {
                  saved.Name = model.Name;
                  saved.College = model.College;
                  saved.LawyerTypeId = model.LawyerTypeId;
                  saved.LawyerStateId = model.LawyerStateId;
                  saved.ModifyDate = DateTime.Now;
              }
              else
              {
                  model.Gid = Guid.NewGuid();
                  model.CreateDate = DateTime.Now;
                  model.ModifyDate = DateTime.Now;
                  await repo.AddAsync(model);
              }
              await repo.SaveChangesAsync();
          }
      }

      */
    }
}

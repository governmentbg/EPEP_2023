using Epep.Core.Constants;
using Epep.Core.Contracts;
using Epep.Core.Data;
using Epep.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace Epep.Core.Services
{
    public class DocumentCleanerService : IDocumentCleanerService
    {

        private readonly IRepository repo;
        private readonly int MaxDraftDays;
        private readonly int MaxNotifiedDays;
        private readonly IEmailService emailService;

        public DocumentCleanerService(
            IRepository _repo,
            IEmailService _emailService,
            IConfiguration config)
        {
            MaxDraftDays = config.GetValue<int>("DocumentMaxDraftDays", 7);
            MaxNotifiedDays = config.GetValue<int>("MaxNotifiedDays", 1);
            repo = _repo;
            emailService = _emailService;
        }


        public async Task Clean()
        {
            DateTime dtFromDeleteBefore = DateTime.Now.AddDays(-MaxNotifiedDays);
            DateTime dtTotalDays = DateTime.Now.AddDays(-(MaxDraftDays+ MaxNotifiedDays));
            var documentsForDelete = await repo.AllReadonly<ElectronicDocument>()
                                               .Where(x => x.ModifyDate < dtTotalDays)
                                               .Where(x => x.DateCleanNotified < dtFromDeleteBefore)
                                               .Where(x => x.DateCleanNotified != null)
                                               .Where(x => x.DateApply == null)
                                               .OrderBy(x => x.Id)
                                               .Select(x => x.Id)
                                               .Take(1000)
                                               .ToListAsync();

            foreach (var documentId in documentsForDelete)
            {
                try
                {
                    await cleanOneDocument(documentId);
                }
                catch (Exception ex) { }
            }
        }

        public async Task Notify()
        {
            DateTime dtFromDeleteBefore = DateTime.Now.Date.AddDays(-MaxDraftDays);
            Expression<Func<ElectronicDocument, bool>> whereNotifyEmails = x => x.ModifyDate < dtFromDeleteBefore
            && x.DateApply == null && x.DateCleanNotified == null;
            var documentsForNotify = await repo.AllReadonly<ElectronicDocument>()
                                               .Where(whereNotifyEmails)
                                               .OrderBy(x => x.Id)
                                               .Select(x => new
                                               {
                                                   x.Id,
                                                   x.CreateUserId,
                                                   x.CreateUser.FullName,
                                                   Email = x.CreateUser.NotificationEmail ?? x.CreateUser.Email
                                               })
                                               .Take(1000)
                                               .ToListAsync();

            foreach (var createUserId in documentsForNotify.Select(x => x.CreateUserId).Distinct())
            {
                try
                {
                    var documentsCount = documentsForNotify.Where(x => x.CreateUserId == createUserId).Count();
                    var user = documentsForNotify.Where(x => x.CreateUserId == createUserId).FirstOrDefault();


                    await emailService.NewMailMessage(user.Email, NomenclatureConstants.EmailTemplates.DocumentCleanMessage,
                            JObject.FromObject(
                    new
                    {
                        userName = user.FullName,
                        draftDays = MaxDraftDays,
                        notifyDays = MaxNotifiedDays,
                        docCount = documentsCount
                    }));

                    var documentsForNotifyUpdate = await repo.All<ElectronicDocument>()
                                                            .Where(whereNotifyEmails)
                                                            .Where(x => x.CreateUserId == user.CreateUserId)
                                                            .ToListAsync();
                    var dtNow = DateTime.Now;
                    foreach (var document in documentsForNotifyUpdate)
                    {
                        document.DateCleanNotified = dtNow;
                    }
                    await repo.SaveChangesAsync();
                }
                catch (Exception ex) { }
            }
        }



        async Task cleanOneDocument(long documentId)
        {
            var document = await repo.GetByIdAsync<ElectronicDocument>(documentId);

            var sides = await repo.All<ElectronicDocumentSide>()
                                    .Include(x => x.Subject)
                                    .ThenInclude(x => x.Person)
                                    .Include(x => x.Subject)
                                    .ThenInclude(x => x.Entity)
                                    .Where(x => x.ElectronicDocumentId == documentId)
                                    .ToListAsync();

            foreach (var side in sides)
            {
                if (side.Subject.Person != null)
                {
                    repo.Delete(side.Subject.Person);
                }
                if (side.Subject.Entity != null)
                {
                    repo.Delete(side.Subject.Entity);
                }
                repo.Delete(side.Subject);
                repo.Delete(side);
            }

            repo.Delete(document);
            await repo.SaveChangesAsync();
        }
    }
}

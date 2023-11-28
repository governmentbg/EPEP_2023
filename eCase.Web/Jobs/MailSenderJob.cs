using System;
using System.Configuration;
using System.Threading;
using System.Linq;
using eCase.Common.Jobs;
using Autofac.Features.OwnedInstances;
using eCase.Common.NLog;
using eCase.Data.Core;
using eCase.Common.Helpers;
using eCase.Components.MailProvider;
using eCase.Data.Repositories;
using RazorEngine.Templating;
using System.Collections.Concurrent;
using eCase.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using eCase.Domain.Emails;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace eCase.Web.Jobs
{
    public class EmailJob : IJob
    {
        private Func<Owned<DisposableTuple<IUnitOfWork, IMailRepository, IMailProvider>>> dependencyFactory;
        private object syncRoot = new object();
        private ILogger logger;
        private IRazorEngineService razorEngineService;
        ConcurrentDictionary<string, string> compiledTemplates;
        private bool disposed;
        private int batchSize;
        private TimeSpan period;
        private int maxFailedAttempts;
        private TimeSpan failedAttemptTimeout;
        private int parallelMailTasks;
        private int successes;
        private int failures;

        public EmailJob(Func<Owned<DisposableTuple<IUnitOfWork, IMailRepository, IMailProvider>>> dependencyFactory, ILogger logger)
        {
            this.dependencyFactory = dependencyFactory;
            this.logger = logger;
            this.razorEngineService = RazorEngineService.Create();
            this.compiledTemplates = new ConcurrentDictionary<string, string>();
            this.disposed = false;

            this.batchSize = int.Parse(ConfigurationManager.AppSettings["eCase.Web.MailJob:EmailJobBatchSize"]);
            this.period = TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["eCase.Web.MailJob:EmailJobPeriodInSeconds"]));
            this.maxFailedAttempts = int.Parse(ConfigurationManager.AppSettings["eCase.Web.MailJob:EmailJobMaxFailedAttempts"]);
            this.failedAttemptTimeout = TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings["eCase.Web.MailJob:EmailJobFailedAttemptTimeoutInMinutes"]));
            this.parallelMailTasks = int.Parse(ConfigurationManager.AppSettings["eCase.Web.MailJob:EmailJobParallelMailTasks"]);
        }

        public string Name
        {
            get { return "EmailJob"; }
        }

        public TimeSpan Period
        {
            get { return this.period; }
        }

        public void Action(CancellationToken ct)
        {
            IList<long> pendingEmailIds = new List<long>();

            try
            {
                if (disposed)
                {
                    return;
                }

                using (var factory = this.dependencyFactory())
                {
                    var unitOfWork = factory.Value.Item1;
                    var emailsRepository = factory.Value.Item2;

                    pendingEmailIds = emailsRepository.GetPendingEmailIds(this.batchSize, this.maxFailedAttempts, this.failedAttemptTimeout);
                }

                if (pendingEmailIds.Any())
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    this.successes = 0;
                    this.failures = 0;

                    SendParallel(ct, pendingEmailIds).Wait();

                    sw.Stop();

                    this.logger.Info(string.Format("Email batch finished in {0}ms - {1} emails send, {2} failures of total {3} emails.", sw.ElapsedMilliseconds, this.successes, this.failures, pendingEmailIds.Count));
                }
            }
            catch (OperationCanceledException ex)
            {
                this.logger.Error(
                    string.Format("Job was canceled due to a token cancellation request; Email batch finished with {1} emails send, {2} failures of total {3} emails. {4}", 
                    this.successes, 
                    this.failures, 
                    pendingEmailIds.Count,
                    Helper.GetDetailedExceptionInfo(ex)));
            }
            catch (Exception ex)
            {
                this.logger.Error(Helper.GetDetailedExceptionInfo(ex));
            }
        }

        private Task SendParallel(CancellationToken ct, IList<long> pendingEmailIds)
        {
            ConcurrentQueue<long> mailIds = new ConcurrentQueue<long>(pendingEmailIds);

            int numberOfParallelTasks = Math.Min(mailIds.Count, this.parallelMailTasks);
            var parallelTasks = Enumerable.Range(0, numberOfParallelTasks)
                .Select(pt => Task.Run(() => Send(ct, mailIds), ct))
                .ToArray();

            return Task.WhenAll(parallelTasks);
        }

        private async Task Send(CancellationToken ct, ConcurrentQueue<long> mailIds)
        {
            long mailId;
            Email email;

            using (var factory = this.dependencyFactory())
            {
                var unitOfWork = factory.Value.Item1;
                var emailsRepository = factory.Value.Item2;
                var mailProvider = factory.Value.Item3;

                while (mailIds.TryDequeue(out mailId))
                {
                    if (disposed)
                    {
                        break;
                    }

                    ct.ThrowIfCancellationRequested();

                    email = emailsRepository.Find(mailId);

                    try
                    {
                        TemplateConfig templateConfig = TemplateConfig.Get(email.MailTemplateName);

                        await mailProvider.SendAsync(
                                email.Recipient,
                                templateConfig.MailSubject,
                                BuildEmailBody(templateConfig.TemplateFileName, email.Context),
                                templateConfig.IsBodyHtml,
                                null,
                                templateConfig.SenderMail,
                                templateConfig.SenderName,
                                null,
                                false);

                        email.SetStatus(EmailStatus.Sent);
                        email.ModifyDate = DateTime.Now;
                        Interlocked.Increment(ref this.successes);
                    }
                    catch (SmtpException smtpEx)
                    {
                        var exception = "SmtpException: " + Enum.GetName(typeof(SmtpStatusCode), smtpEx.StatusCode);
                        email.IncrementFailedAttempts(exception);
                        this.logger.Warn(exception);
                        Interlocked.Increment(ref this.failures);
                    }
                    catch (Exception ex)
                    {
                        email.SetStatus(EmailStatus.UknownError);
                        email.IncrementFailedAttempts(ex.Message);
                        this.logger.Error(Helper.GetDetailedExceptionInfo(ex));
                        Interlocked.Increment(ref this.failures);
                    }

                    unitOfWork.Save();
                }
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.razorEngineService.Dispose();
                this.razorEngineService = null;

                this.disposed = true;

                this.logger.Info("Email job disposed");
            }
        }

        private string BuildEmailBody(string templateFileName, string context)
        {
            if (!this.compiledTemplates.ContainsKey(templateFileName))
            {
                lock (this.syncRoot)
                {
                    if (!this.compiledTemplates.ContainsKey(templateFileName))
                    {
                        string templatePath = GetTemplatePath(templateFileName);
                        string razorTemplate = File.ReadAllText(templatePath);
                        this.razorEngineService.Compile(razorTemplate, templateFileName, typeof(JObject));
                        this.compiledTemplates.TryAdd(templateFileName, templateFileName);
                    }
                }
            }

            return this.razorEngineService.Run(templateFileName, null, context != null ? JObject.Parse(context) : null);
        }

        private string GetTemplatePath(string templateName)
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string binPath = System.IO.Path.GetDirectoryName(rootPath);
            string templateFullPath = String.Format(@"{0}\Jobs\Templates\{1}", binPath, templateName);

            return templateFullPath;
        }
    }

    //public class MailSenderJob : IJob
    //{
    //    private Func<Owned<ILogger>> loggerFactory; 
    //    private Func<Owned<IUnitOfWork>> unitOfWorkFactory;
    //    private Func<Owned<IMailProvider>> mailProviderFactory;
    //    private Func<Owned<IMailRepository>> mailRepositoryFactory;

    //    public MailSenderJob
    //        (
    //            Func<Owned<ILogger>> loggerFactory,
    //            Func<Owned<IUnitOfWork>> unitOfWorkFactory,
    //            Func<Owned<IMailProvider>> mailProviderFactory,
    //            Func<Owned<IMailRepository>> mailRepositoryFactory
    //        )
    //    {
    //        this.loggerFactory = loggerFactory;
    //        this.unitOfWorkFactory = unitOfWorkFactory;
    //        this.mailProviderFactory = mailProviderFactory;
    //        this.mailRepositoryFactory = mailRepositoryFactory;
    //    }

    //    public string Name
    //    {
    //        get { return "MailSenderJob"; }
    //    }

    //    public TimeSpan Period
    //    {
    //        get
    //        {
    //            return TimeSpan.FromMinutes(
    //                int.Parse(ConfigurationManager.AppSettings["eCase.Web:MailSenderJobIntervalInMinutes"]));
    //        }
    //    }

    //    public void Action()
    //    {
    //        try
    //        {
    //            using (var mailRepository = mailRepositoryFactory())
    //            {
    //                using (var mailProvider = mailProviderFactory())
    //                {
    //                    //var pendingIds = mailRepository.Value.GetPendingEmailIds().ToList();
    //                    //
    //                    //foreach (var mailId in pendingIds)
    //                    //{
    //                    //    var senderEmail = ConfigurationManager.AppSettings["eCase.Web:SenderEmail"];
    //                    //    var senderName = ConfigurationManager.AppSettings["eCase.Web:SenderName"];
    //                    //
    //                    //    if (mailProvider.Value.Send(message.Recipient, message.Subject, message.Message,
    //                    //            message.IsBodyHtml, null, senderEmail, senderName, null, false))
    //                    //    {
    //                    //        using (var unitOfWork = unitOfWorkFactory())
    //                    //        {
    //                    //            var u_message = mailRepository.Value.GetMessagesById(message.MailQueueId);
    //                    //    
    //                    //            u_message.SentDate = DateTime.Now;
    //                    //    
    //                    //            unitOfWork.Value.Save();
    //                    //        }
    //                    //    }
    //                    //}
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            using (var logger = loggerFactory())
    //            {
    //                logger.Value.Error(string.Format("{0}: {1}", this.Name, Helper.GetDetailedExceptionInfo(ex)));
    //            }
    //        }
    //    }
    //}
}
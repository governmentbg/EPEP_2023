using Autofac;
using Autofac.Configuration;
using Autofac.Extras.Attributed;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using eCase.Common.Helpers;
using eCase.Common.Jobs;
using eCase.Common.NLog;
using eCase.Components.EventHandlers;
using eCase.Components.MailProvider;
using eCase.Data;
using eCase.Domain;
using eCase.Web.Jobs;
using NLog.Internal;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;


namespace eCase.Web.App_Start
{
    public class InitialConfig
    {
        public static void Init()
        {
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            IContainer container = BuildContainer();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            System.Web.Helpers.AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;

            StartJobs(container);
        }

        public static void StartJobs(IContainer container)
        {
            var jobs = container.Resolve<IJob[]>();

            foreach (var job in jobs)
            {
                //(new JobHost(job)).Start();
                container.Resolve<JobHost>().Start(job, new CancellationToken(false));
            }
        }

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).WithAttributeFilter();
            builder.RegisterApiControllers(typeof(MvcApplication).Assembly).WithAttributeFilter();
            //builder.RegisterType(typeof(eCase.Web.Controllers.PublicController)).InstancePerLifetimeScope();
            //builder.RegisterApiControllers(typeof(eCase.Web.Controllers.PublicController).Assembly);

            builder.RegisterModule(new ConfigurationSettingsReader());

            builder.RegisterModule(new EventHandlersModule());
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DataModule());
            builder.RegisterModule(new LogModule());
            builder.RegisterModule(new MailModule());

            var emailJobEnabled = (System.Configuration.ConfigurationManager.AppSettings["eCase.Service:EmailJobEnabled"] ?? "true") == "true";

            if (emailJobEnabled)
            {
                builder.RegisterType<JobHost>();
                builder.RegisterType<EmailJob>().As<IJob>();
            }
            return builder.Build();
        }
    }
}
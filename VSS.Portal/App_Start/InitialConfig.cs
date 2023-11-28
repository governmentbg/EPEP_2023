using System.Threading;
using Autofac;
using Autofac.Extras.Attributed;
using Autofac.Integration.Mvc;
using eCase.Common.Helpers;
using eCase.Common.Jobs;
using eCase.Common.NLog;
using eCase.Components.EventHandlers;
using eCase.Components.MailProvider;
using eCase.Data;
using eCase.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VSS.Portal.App_Start
{
    public class InitialConfig
    {
        public static void Init()
        {
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            IContainer container = BuildContainer();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

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
            //builder.RegisterApiControllers(typeof(MvcApplication).Assembly).WithAttributeFilter();
            //builder.RegisterType(typeof(eCase.Web.Controllers.PublicController)).InstancePerLifetimeScope();
            //builder.RegisterApiControllers(typeof(eCase.Web.Controllers.PublicController).Assembly);

            //builder.RegisterModule(new ConfigurationSettingsReader());

            builder.RegisterModule(new EventHandlersModule());
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DataModule());
            builder.RegisterModule(new LogModule());
            builder.RegisterModule(new MailModule());

            return builder.Build();
        }
    }
}
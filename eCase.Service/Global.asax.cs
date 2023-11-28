using Autofac;
using Autofac.Integration.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using eCase.Data;
using eCase.Domain;
using eCase.Domain.Service;
using eCase.Common.NLog;
using eCase.Components.EventHandlers;
using Autofac.Extras.Attributed;

namespace eCase.Service
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterModule(new EventHandlersModule());
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new DataModule());
            builder.RegisterModule(new LogModule());

            builder.RegisterType<eCaseService>().As<IeCaseService>().WithAttributeFilter();

            AutofacHostFactory.Container = builder.Build();
        }
    }
}
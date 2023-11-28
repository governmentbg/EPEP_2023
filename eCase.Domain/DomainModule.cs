using Autofac;
using eCase.Common.Db;
using eCase.Domain.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCase.Domain
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<DbConfiguration>().As<IDbConfiguration>().SingleInstance();

            moduleBuilder.RegisterType<EmailsModelDbConfiguration>().As<IDbConfiguration>().SingleInstance();
        }
    }
}

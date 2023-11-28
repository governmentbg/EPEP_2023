using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCase.Components.Communicators.HelpDeskCommunicator
{
    class HelpDeskModule : Module
    {
        public bool IsFake { get; set; }
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            if (IsFake)
            {
                moduleBuilder.RegisterType<FakeHelpDeskCommunicator>().As<IHelpDeskCommunicator>().InstancePerLifetimeScope();
            }
            else
            {
                moduleBuilder.RegisterType<HelpDeskCommunicator>().As<IHelpDeskCommunicator>().InstancePerLifetimeScope();
            }
        }
    }
}

using Autofac;

using eCase.Domain.Core;

namespace eCase.Components.EventHandlers
{
    public class EventHandlersModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            //Domain Event Handlers
            moduleBuilder.RegisterType<NewRegistrationHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ForgottenPasswordHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SummonNotificationHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SummonAccessActivationHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<SummonAccessDeactivationHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<FeedbackHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<CaseAccessHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ChangeUserNameHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ChangeCaseAccessHandler>().As<IEventHandler>().InstancePerLifetimeScope();
            moduleBuilder.RegisterType<ChangeUserProfileHandler>().As<IEventHandler>().InstancePerLifetimeScope();
        }
    }
}

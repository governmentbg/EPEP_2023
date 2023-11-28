using Autofac;
namespace eCase.Components.MailProvider
{
    public class MailModule : Module
    {
        protected override void Load(ContainerBuilder moduleBuilder)
        {
            moduleBuilder.RegisterType<VSSProviderImpl>().As<IMailProvider>().InstancePerDependency();
        }
    }
}
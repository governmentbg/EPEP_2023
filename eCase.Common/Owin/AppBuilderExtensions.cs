using System.Threading;
using Autofac;
using Microsoft.Owin;
using Owin;

namespace eCase.Common.Owin
{
    public static class AppBuilderExtensions
    {
        public static void DisposeContainerOnShutdown(this IAppBuilder app, IContainer container)
        {
            var context = new OwinContext(app.Properties);
            var token = context.Get<CancellationToken>("host.OnAppDisposing");

            if (token != CancellationToken.None)
            {
                token.Register(container.Dispose);
            }
        }
    }
}

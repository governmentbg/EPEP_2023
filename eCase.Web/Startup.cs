using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eCase.Web.Startup))]
namespace eCase.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

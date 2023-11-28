using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(VSS.Portal.Startup))]

namespace VSS.Portal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

        }
    }
}

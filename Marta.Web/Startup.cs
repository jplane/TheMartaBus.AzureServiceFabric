
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Marta.Web.Startup))]

namespace Marta.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}

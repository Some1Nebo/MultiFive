using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MultiFive.Web.Startup))]
namespace MultiFive.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dBAccess.Startup))]
namespace dBAccess
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

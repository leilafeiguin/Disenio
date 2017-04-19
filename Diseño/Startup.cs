using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Diseño.Startup))]
namespace Diseño
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

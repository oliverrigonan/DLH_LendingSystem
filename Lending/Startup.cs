using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lending.Startup))]
namespace Lending
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

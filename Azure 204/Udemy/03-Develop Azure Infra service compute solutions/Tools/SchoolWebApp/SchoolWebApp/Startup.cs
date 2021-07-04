using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SchoolWebApp.Startup))]
namespace SchoolWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Wedding.Startup))]
namespace Wedding
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

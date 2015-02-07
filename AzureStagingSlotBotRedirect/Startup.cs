using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureStagingSlotBotRedirect.Startup))]
namespace AzureStagingSlotBotRedirect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

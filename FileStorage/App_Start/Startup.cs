using BLL.Config;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(FileStorage.App_Start.Startup))]
namespace FileStorage.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            BusinessLogicLayerAppBuilderConfiguration.Configure(app);
        }
    }
}
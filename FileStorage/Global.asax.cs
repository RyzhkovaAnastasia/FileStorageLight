using BLL.Config;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Mvc;
using NLog;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FileStorage
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            NinjectDIConfig();
        }

        protected void Application_Error()
        {
            var ex = Server.GetLastError();
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Error(ex);
        }

        private void NinjectDIConfig()
        {
            var BLLDI = new BusinessLogicLayerDIConfig();

            INinjectModule[] registrations = { BLLDI, BLLDI.DLLDI };

            var kernel = new StandardKernel(registrations);
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }

      
    }
}

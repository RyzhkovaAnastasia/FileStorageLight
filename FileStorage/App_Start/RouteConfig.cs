using System.Web.Mvc;
using System.Web.Routing;

namespace FileStorage
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               name: "Share",
               url: "sharing/{link}",
               new { controller = "SharedFileLink", action = "SharedByLink"}
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Storage", action = "Index", id = UrlParameter.Optional }
            );

           
        }
    }
}

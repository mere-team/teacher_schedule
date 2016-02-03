using System.Web.Mvc;
using System.Web.Routing;

namespace Schedule
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "UlstuStudent",
                url: "ulstu/student/{action}/{id}",
                defaults: new {controller = "UlstuStudent", id = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "UlstuTeacher",
                url: "ulstu/teacher/{action}/{id}",
                defaults: new {controller = "UlstuTeacher", id = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

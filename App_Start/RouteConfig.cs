using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "display",
                url: "display/{ip}/{port}/{rate}",
                defaults: new { controller = "First", action = "display", ip = UrlParameter.Optional, port = UrlParameter.Optional, rate = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "save",
               url: "save/{ip}/{port}/{rate}/{seconds}/{file}",
               defaults: new { controller = "First", action = "save", ip = UrlParameter.Optional, port = UrlParameter.Optional, rate = UrlParameter.Optional, seconds = UrlParameter.Optional, file = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "First", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
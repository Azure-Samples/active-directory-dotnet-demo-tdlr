using System.Web.Mvc;
using System.Web.Routing;

namespace Tdlr
{
    public class RouteConfig
    {
        // Register the Web App Routes
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("aad", "account/signin/aad", new { controller = "Account", action = "AADSignIn" });

            routes.MapRoute("aadsignup", "account/signup/aad", new { controller = "Account", action = "AADSignUp" });

            routes.MapRoute("create", "tasks",
                new { controller = "Tasks", action = "Create" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional});
        }
    }
}
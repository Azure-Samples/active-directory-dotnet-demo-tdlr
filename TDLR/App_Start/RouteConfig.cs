using System.Web.Mvc;
using System.Web.Routing;

namespace Tdlr
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("aad", "account/signin/aad", new { controller = "Account", action = "AADSignIn" });

            routes.MapRoute("create", "tasks",
                new { controller = "Tasks", action = "Create" },
                new { httpMethod = new HttpMethodConstraint("POST") });

            routes.MapRoute("get", "tasks/{id}",
                new { controller = "Tasks", action = "Get" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("update", "tasks/{id}",
                new { controller = "Tasks", action = "Update" },
                new { httpMethod = new HttpMethodConstraint("PATCH") });

            routes.MapRoute("delete", "tasks/{id}",
                new { controller = "Tasks", action = "Delete" },
                new { httpMethod = new HttpMethodConstraint("DELETE") });

            routes.MapRoute("updateshares", "tasks/{id}/share",
                new { controller = "Tasks", action = "UpdateShares" },
                new { httpMethod = new HttpMethodConstraint("PATCH") });

            routes.MapRoute("getshares", "tasks/{id}/share",
                new { controller = "Tasks", action = "GetShares" },
                new { httpMethod = new HttpMethodConstraint("GET") });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}
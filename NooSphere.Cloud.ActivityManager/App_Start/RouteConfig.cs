using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using NooSphere.Cloud.ActivityManager.Events;
using SignalR;

namespace NooSphere.Cloud.ActivityManager.App_Start
{
    public class RouteConfig
    {

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            // Persistent Event Connections
            routes.MapConnection<EventDispatcher>(
                name: "Api.Connect",
                url: "Api/Connect/{*operation}"
            );

            // REST Api routing (Activities)
            routes.MapHttpRoute(
                name: "Api.Activities",
                routeTemplate: "Api/Activities/{activityId}",
                defaults: new { controller = "Activity", activityId = RouteParameter.Optional }
            );

            // REST Api routing (Subscribe)
            routes.MapHttpRoute(
                name: "Api.Activities.Subscribe",
                routeTemplate: "Api/Activities/{activityId}/Subscribe",
                defaults: new { controller = "Activity", action = "Subscribe" }
            );

            // REST Api routing (Unsubscribe)
            routes.MapHttpRoute(
                name: "Api.Activities.Unsubscribe",
                routeTemplate: "Api/Activities/{activityId}/Unsubscribe",
                defaults: new { controller = "Activity", action = "Unsubscribe" }
            );

            // REST Api routing (Actions)
            routes.MapHttpRoute(
                name: "Api.Activities.Actions",
                routeTemplate: "Api/Activities/{activityId}/Actions/{actionId}",
                defaults: new { controller = "Action", actionId = RouteParameter.Optional }
            );

            // REST Api routing (Resources)
            routes.MapHttpRoute(
                name: "Api.Resources",
                routeTemplate: "Api/Activities/{activityId}/Actions/{actionId}/Resources/{resourceId}",
                defaults: new { controller = "File", resourceId = RouteParameter.Optional }
            );

            // REST Api routing (Users)
            routes.MapHttpRoute(
                name: "Api.Users",
                routeTemplate: "Api/Users/{userId}",
                defaults: new { controller = "User", userId = RouteParameter.Optional }
            );

            // REST Api routing (Login)
            routes.MapHttpRoute(
                name: "Api.Users.Login",
                routeTemplate: "Api/Login/{email}",
                defaults: new { controller = "User", action = "Login" }
            );

            // REST Api routing (Logout)
            routes.MapHttpRoute(
                name: "Api.Users.Logout",
                routeTemplate: "Api/Logout/{email}",
                defaults: new { controller = "User", action = "Logout" }
            );

            // REST Api routing (For testing purposes)
            routes.MapHttpRoute(
                name: "Api.Test",
                routeTemplate: "Api/Test/{action}",
                defaults: new { controller = "Test" }
            );

            // Website routing
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
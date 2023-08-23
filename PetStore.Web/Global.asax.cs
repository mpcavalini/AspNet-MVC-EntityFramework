using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Serilog;

namespace PetStore.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var pathLog = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs/log.txt");
            Log.Logger = new LoggerConfiguration()
               .WriteTo.File(pathLog, rollingInterval: RollingInterval.Hour)
               .CreateLogger();
        }

        protected void Application_PreSendRequestHeaders()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Headers.Remove("Server");
            }
        }

        protected void Application_End()
        {
            Log.CloseAndFlush();
        }
    }
}

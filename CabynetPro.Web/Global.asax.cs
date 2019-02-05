using System.Web.Mvc;
using System.Web.Routing;
using CabynetPro.Core.Utility;

namespace CabynetPro.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ActivityLogger.LogFileName = "Cabynet_System_Logs.txt";
        }
    }
}

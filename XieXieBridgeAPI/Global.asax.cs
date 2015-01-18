using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace XieXieBridgeAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static Wifi Wifi;
        protected void Application_Start()
        {
            SerialConnectionSingleton.Instance.Setup();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Wifi = new Wifi();
        }
    }
}

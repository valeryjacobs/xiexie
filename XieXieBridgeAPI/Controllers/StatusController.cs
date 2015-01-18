using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XieXieBridgeAPI.Models;
using System.Windows.Forms;
using System.Net.Sockets;
using SimpleWifi;

namespace XieXieBridgeAPI.Controllers
{
    public class StatusController : ApiController
    {
       
        public Status Get()
        {
            var accessPoint = WebApiApplication.Wifi.GetAccessPoints().Where(x => x.IsConnected == true).Single();

            return new Status()
            {
                BatteryLevel = Convert.ToInt32(SystemInformation.PowerStatus.BatteryLifePercent*100),
                Charging = SystemInformation.PowerStatus.BatteryChargeStatus.ToString().Contains("Charging"),
                IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(),
                PublicIPAddress = GetPublicIP(),
                WifiConnection = accessPoint.Name,
                WifiReception = Convert.ToInt32(accessPoint.SignalStrength)
            };

        }

        private static string GetPublicIP()
        {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
    }
}

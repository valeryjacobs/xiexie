﻿using System;
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
            AccessPoint accessPoint = null;

            try
            {
                accessPoint = WebApiApplication.Wifi.GetAccessPoints().Where(x => x.IsConnected == true).Single();
            }
            catch { }

            var ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
            var publicIPAddress = GetPublicIP();



            return new Status()
            {
                BatteryLevel = Convert.ToInt32(SystemInformation.PowerStatus.BatteryLifePercent * 100),
                Charging = SystemInformation.PowerStatus.BatteryChargeStatus.ToString().Contains("Charging"),
                IPAddress = ipAddress,
                PublicIPAddress = publicIPAddress,
                WifiConnection = accessPoint != null ? accessPoint.Name : "Inactive!",
                WifiReception = accessPoint != null ? Convert.ToInt32(accessPoint.SignalStrength) : 0
            };

        }

        private static string GetPublicIP()
        {
            System.Net.WebResponse resp = null;
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);

            try
            {
                resp = req.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                string response = sr.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                string a4 = a3[0];
                return a4;
            }
            catch { }

            return "Not active!";
          
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XieXieBridgeAPI.Models
{
    public class Status
    {
        public string IPAddress { get; set; }
        public string PublicIPAddress { get; set; }
        public int BatteryLevel { get; set; }
        public int MotorBatteryLevel { get; set; }
        public bool Charging { get; set; }
        public int WifiReception { get; set; }
        public string WifiConnection { get; set; }
    }
}

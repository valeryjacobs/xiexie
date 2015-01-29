using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XieXieBridgeAPI.Models
{
    public class Command
    {
        public int Target { get; set; }
        public int CommandType { get; set; }
        public string[] Params { get; set; }
    }
}
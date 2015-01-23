using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XieXieBridgeAPI.Models;

namespace XieXieBridgeAPI.Controllers
{
    public class ConfigController : ApiController
    {
        public Config Get()
        {
            return new Config() {  PeerId = "XieXieCon4"};
        }
    }
}

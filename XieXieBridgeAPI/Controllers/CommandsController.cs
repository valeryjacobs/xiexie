using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XieXieBridgeAPI.Models;

namespace XieXieBridgeAPI.Controllers
{
    public class CommandsController : ApiController
    {

        public IEnumerable<Command> GetAll()
        {
         
            var commands = new List<Command> { new Command { CommandType = (int)CommandType.SetAll }, new Command {CommandType = (int)CommandType.SetAllOff } };
            return commands;
        }

        public HttpResponseMessage PostCommand(Command command)
        {

            var response = Request.CreateResponse<Command>(HttpStatusCode.Created, command);

            SerialConnectionSingleton.Instance.Execute((CommandType)command.CommandType,command.Params);

            return response;
        }
    }
}

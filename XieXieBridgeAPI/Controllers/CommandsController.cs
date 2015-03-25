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

            var commands = new List<Command> { new Command { CommandType = (int)FourWDCommandType.SetAll }, new Command { CommandType = (int)FourWDCommandType.SetAllOff } };
            return commands;
        }

        public HttpResponseMessage PostCommand(Command command)
        {
            var response = Request.CreateResponse<Command>(HttpStatusCode.Created, command);

            if (command.Target == 0)
            {
                SerialConnectionSingleton.Instance.ExecuteFourWD((FourWDCommandType)command.CommandType, command.Params);

            }
            else if (command.Target == 1)
            {
                SerialConnectionSingleton.Instance.ExecuteHead((HeadCommandType)command.CommandType, command.Params);
            }

            return response;
        }
    }
}

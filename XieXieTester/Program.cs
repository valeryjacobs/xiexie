using CommandMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XieXieBridgeAPI;

namespace XieXieTester
{
    class Program
    {
        static void Main(string[] args)
        {
            SerialConnectionSingleton.Instance.Setup();
            //SerialConnectionSingleton.Instance.Execute(CommandType.SetAll,null);

            SerialConnectionSingleton.Instance.Messenger.SendCommand(new SendCommand(1, 100));
            SerialConnectionSingleton.Instance.Messenger.SendCommand(new SendCommand(1, 100));

           // this._cmdMessenger.SendCommand(new SendCommand(1, 100));
        }
    }
}

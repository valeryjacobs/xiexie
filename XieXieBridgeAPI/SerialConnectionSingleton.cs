using CommandMessenger;
using CommandMessenger.TransportLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace XieXieBridgeAPI
{
    public enum FourWDCommandType
    {
        SetLed = 0,
        SetAll = 1,
        SetAllOff = 2,
        TurnL = 3,
        TurnR = 4,
        Stop = 5,
        SetSpeed = 6,
        Status = 7
    };

    public enum HeadCommandType
    {
        Pan = 0,
        Tilt = 1,
        LightOn = 2,
        LightOff = 3,
        Light = 4,
        Status = 5
    }

    public sealed class SerialConnectionSingleton
    {
        private static readonly SerialConnectionSingleton instance = new SerialConnectionSingleton();

        private SerialConnectionSingleton() { }

        public static SerialConnectionSingleton Instance
        {
            get
            {
                return instance;
            }
        }

        public void ExecuteFourWD(FourWDCommandType command, object parameter)
        {
            switch (command)
            {
                case FourWDCommandType.SetAllOff:
                case FourWDCommandType.Stop:
                case FourWDCommandType.TurnL:
                case FourWDCommandType.TurnR:
                case FourWDCommandType.SetLed:
                    FourWDMessenger.SendCommand(new SendCommand((int)command));
                    break;
                case FourWDCommandType.SetAll:
                    FourWDMessenger.SendCommand(new SendCommand((int)command, int.Parse(((string[])parameter)[0])));
                    break;
            }
        }

        public void ExecuteHead(HeadCommandType command, object parameter)
        {
            switch (command)
            {
                case HeadCommandType.Pan:
                    HeadMessenger.SendCommand(new SendCommand((int)command, int.Parse(((string[])parameter)[0])));
                    break;
                case HeadCommandType.Tilt:
                    HeadMessenger.SendCommand(new SendCommand((int)command, int.Parse(((string[])parameter)[0])));
                    break;
                case HeadCommandType.LightOn:
                    HeadMessenger.SendCommand(new SendCommand((int)command));
                    break;
                case HeadCommandType.LightOff:
                    HeadMessenger.SendCommand(new SendCommand((int)command));
                    break;
                case HeadCommandType.Light:
                    HeadMessenger.SendCommand(new SendCommand((int)command, int.Parse(((string[])parameter)[0])));
                    break;
            }
        }

        public void Setup()
        {
            FourWDTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = System.Configuration.ConfigurationManager.AppSettings["arduino4WDPort"], BaudRate = 115200, DtrEnable = false } // object initializer
            };

            HeadTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = System.Configuration.ConfigurationManager.AppSettings["arduinoHeadPort"], BaudRate = 115200, DtrEnable = false } // object initializer
            };

            FourWDMessenger = new CmdMessenger(FourWDTransport)
            {
                BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
            };

            HeadMessenger = new CmdMessenger(HeadTransport)
            {
                BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
            };

            FourWDMessenger.Attach(OnUnknownCommand);
            FourWDMessenger.Attach((int)FourWDCommandType.Status, OnStatus);

            FourWDMessenger.StartListening();


            HeadMessenger.Attach(OnUnknownCommand);
            HeadMessenger.Attach((int)FourWDCommandType.Status, OnStatus);

            HeadMessenger.StartListening();

            Debug.WriteLine("Connection established.");
        }

        void OnUnknownCommand(ReceivedCommand arguments)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(arguments));
            Debug.WriteLine("Command without attached callback received");
        }

        void OnStatus(ReceivedCommand arguments)
        {
            Debug.Write("Arduino status: ");
            Debug.WriteLine(arguments.ReadStringArg());
        }

        public SerialTransport FourWDTransport { get; set; }
        public SerialTransport HeadTransport { get; set; }
        public CmdMessenger FourWDMessenger { get; set; }
        public CmdMessenger HeadMessenger { get; set; }
    }
}
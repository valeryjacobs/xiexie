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
    public enum CommandType
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

        public void Execute(CommandType command, object parameter)
        {
            switch (command)
            {
                case CommandType.SetAllOff:
                case CommandType.Stop:
                case CommandType.TurnL:
                case CommandType.TurnR:
                case CommandType.SetLed:
                    Messenger.SendCommand(new SendCommand((int)command));
                    break;
                case CommandType.SetAll:
                    Messenger.SendCommand(new SendCommand((int)command, int.Parse(((string[])parameter)[0])));
                    break;
            }
        }

        public void Setup()
        {
            Transport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = "COM5", BaudRate = 115200, DtrEnable = false } // object initializer
            };

            Messenger = new CmdMessenger(Transport)
            {
                BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
            };

            Messenger.Attach(OnUnknownCommand);
            Messenger.Attach((int)CommandType.Status, OnStatus);

            Messenger.StartListening();
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

        public SerialTransport Transport { get; set; }
        public CmdMessenger Messenger { get; set; }
    }
}
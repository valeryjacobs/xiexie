using CommandMessenger;
using CommandMessenger.TransportLayer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class SendAndReceive
    {
        private static void Main()
        {
            SendAndReceive sendAndReceive = new SendAndReceive
            {
                RunLoop = true
            };
            sendAndReceive.Setup();
            while (sendAndReceive.RunLoop)
            {
                sendAndReceive.Loop();
            }
            sendAndReceive.Exit();
        }

        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        private bool state = false;
        public bool RunLoop
        {
            get;
            set;
        }
        public void Setup()
        {
            Console.WriteLine("Setting up connection...");
            this._serialTransport = new SerialTransport
            {
                CurrentSerialSettings =
                {
                    PortName = "COM5",
                    BaudRate = 115200,
                    DtrEnable = false
                }
            };
            this._cmdMessenger = new CmdMessenger(this._serialTransport)
            {
                BoardType = BoardType.Bit16
            };
            this.AttachCommandCallBacks();
            this._cmdMessenger.StartListening();
            Console.WriteLine("Connection established.");
        }
        public void Loop()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Spacebar:
                    Console.Write("STOP");
                    this._cmdMessenger.SendCommand(new SendCommand(5));
                    break;
                case ConsoleKey.PageUp:
                case ConsoleKey.PageDown:
                case ConsoleKey.End:
                case ConsoleKey.Home:
                    break;
                case ConsoleKey.LeftArrow:
                    Console.Write("Left");
                    this._cmdMessenger.SendCommand(new SendCommand(3));
                    break;
                case ConsoleKey.UpArrow:
                    Console.Write("Up");
                    this._cmdMessenger.SendCommand(new SendCommand(1, 100));
                    break;
                case ConsoleKey.RightArrow:
                    Console.Write("Right");
                    this._cmdMessenger.SendCommand(new SendCommand(4));
                    break;
                case ConsoleKey.DownArrow:
                    Console.Write("Down");
                    this._cmdMessenger.SendCommand(new SendCommand(1, -100));
                    break;
                default:
                    switch (key)
                    {
                        case ConsoleKey.D1:
                            Console.Write("Speed 20%");
                            this._cmdMessenger.SendCommand(new SendCommand(6, 50));
                            break;
                        case ConsoleKey.D2:
                            Console.Write("Speed 40%");
                            this._cmdMessenger.SendCommand(new SendCommand(6, 100));
                            break;
                        case ConsoleKey.D3:
                            Console.Write("Speed 60%");
                            this._cmdMessenger.SendCommand(new SendCommand(6, 160));
                            break;
                        case ConsoleKey.D4:
                            Console.Write("Speed 100%");
                            this._cmdMessenger.SendCommand(new SendCommand(6, 200));
                            break;
                        default:
                            if (key == ConsoleKey.X)
                            {
                                this.Exit();
                            }
                            break;
                    }
                    break;
            }
        }
        public void Exit()
        {
            this._cmdMessenger.StopListening();
            this._cmdMessenger.Dispose();
            this._serialTransport.Dispose();
        }
        private void AttachCommandCallBacks()
        {
            this._cmdMessenger.Attach(new CmdMessenger.MessengerCallbackFunction(this.OnUnknownCommand));
            this._cmdMessenger.Attach(7, new CmdMessenger.MessengerCallbackFunction(this.OnStatus));
        }
        private void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine(JsonConvert.SerializeObject(arguments));
            Console.WriteLine("Command without attached callback received");
        }
        private void OnStatus(ReceivedCommand arguments)
        {
            Console.Write("Arduino status: ");
            Console.WriteLine(arguments.ReadStringArg());
        }
    }
}

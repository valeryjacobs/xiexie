using CommandMessenger;
using CommandMessenger.TransportLayer;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class SendAndReceive
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "HostName=vjdevicehub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=6TiIt5wT/2KteegR7ZW7+m0qPT86U6TpIqoKsFtqOCU=";
        static string deviceKey = "8wj7OyIJgtJQFgTvgjIJAz+IC+pLNIHabypMBPbGHUk=";


        private static void Main()
        {
            deviceClient = DeviceClient.Create("vjdevicehub.azure-devices.net", new DeviceAuthenticationWithRegistrySymmetricKey("QuasiRobo", deviceKey));

            ReceiveC2DAsync();

            SendAndReceive sendAndReceive = new SendAndReceive
            {
                RunLoop = true
            };
            Setup();
            while (sendAndReceive.RunLoop)
            {
                Loop();
            }
            sendAndReceive.Exit();
        }

        private static SerialTransport _serialTransport;
        private static CmdMessenger _cmdMessenger;
        private static bool state = false;
        public bool RunLoop
        {
            get;
            set;
        }
        public static void Setup()
        {
            Console.WriteLine("Setting up connection...");
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings =
                {
                    PortName = ConfigurationManager.AppSettings["comPortName"],
                    BaudRate = 115200,
                    DtrEnable = false
                }
            };
            _cmdMessenger = new CmdMessenger(_serialTransport)
            {
                BoardType = BoardType.Bit16
            };
            AttachCommandCallBacks();
            _cmdMessenger.StartListening();
            Console.WriteLine("Connection established.");
        }
        public static void Loop()
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Spacebar:
                    Console.Write("STOP");
                    _cmdMessenger.SendCommand(new SendCommand(5));
                    break;
                case ConsoleKey.PageUp:
                case ConsoleKey.PageDown:
                case ConsoleKey.End:
                case ConsoleKey.Home:
                    break;
                case ConsoleKey.LeftArrow:
                    Console.Write("Left");
                    _cmdMessenger.SendCommand(new SendCommand(3));
                    break;
                case ConsoleKey.UpArrow:
                    Console.Write("Up");
                    _cmdMessenger.SendCommand(new SendCommand(1, int.Parse(ConfigurationManager.AppSettings["power"])));
                    break;
                case ConsoleKey.RightArrow:
                    Console.Write("Right");
                    _cmdMessenger.SendCommand(new SendCommand(4));
                    break;
                case ConsoleKey.DownArrow:
                    Console.Write("Down");
                    _cmdMessenger.SendCommand(new SendCommand(1, -1 * int.Parse(ConfigurationManager.AppSettings["power"])));
                    break;
                case ConsoleKey.L:
                    Console.Write("LightOn");
                    _cmdMessenger.SendCommand(new SendCommand(1, 255));
                    break;
                case ConsoleKey.D1:
                    Console.Write("1");
                    _cmdMessenger.SendCommand(new SendCommand(2));
                    break;
                case ConsoleKey.D2:
                    Console.Write("2");
                    _cmdMessenger.SendCommand(new SendCommand(3));
                    break;
                case ConsoleKey.D3:
                    Console.Write("3");
                    _cmdMessenger.SendCommand(new SendCommand(0,60));
                    break;
                case ConsoleKey.D4:
                    Console.Write("4");
                    _cmdMessenger.SendCommand(new SendCommand(1,60));
                    break;

                default:
                    //switch (key)
                    //{
                    //    case ConsoleKey.D1:
                    //        Console.Write("Speed 20%");
                    //        _cmdMessenger.SendCommand(new SendCommand(6, 50));
                    //        break;
                    //    case ConsoleKey.D2:
                    //        Console.Write("Speed 40%");
                    //        _cmdMessenger.SendCommand(new SendCommand(6, 100));
                    //        break;
                    //    case ConsoleKey.D3:
                    //        Console.Write("Speed 60%");
                    //        _cmdMessenger.SendCommand(new SendCommand(6, 160));
                    //        break;
                    //    case ConsoleKey.D4:
                    //        Console.Write("Speed 100%");
                    //        _cmdMessenger.SendCommand(new SendCommand(6, 200));
                    //        break;
                    //    default:
                    //        if (key == ConsoleKey.X)
                    //        {
                    //            Exit();
                    //        }
                    //        break;
                    //}
                    break;
            }
        }
        public void Exit()
        {
            _cmdMessenger.StopListening();
            _cmdMessenger.Dispose();
            _serialTransport.Dispose();
        }
        private static void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(new CmdMessenger.MessengerCallbackFunction(OnUnknownCommand));
            _cmdMessenger.Attach(7, new CmdMessenger.MessengerCallbackFunction(OnStatus));
            _cmdMessenger.Attach(3, new CmdMessenger.MessengerCallbackFunction(OnStatus));
            _cmdMessenger.Attach(4, new CmdMessenger.MessengerCallbackFunction(OnStatus));
            _cmdMessenger.Attach(1, new CmdMessenger.MessengerCallbackFunction(OnStatus));
            _cmdMessenger.Attach(2, new CmdMessenger.MessengerCallbackFunction(OnStatus));
        }
        private static void OnUnknownCommand(ReceivedCommand arguments)
        {
            Console.WriteLine(JsonConvert.SerializeObject(arguments));
            Console.WriteLine("Command without attached callback received");
        }
        private static void OnStatus(ReceivedCommand arguments)
        {
            Console.Write("Arduino status: ");
            Console.WriteLine(arguments.ReadStringArg());
        }

        private static async void ReceiveC2DAsync()
        {
            Console.WriteLine("\nReceiving cloud to device messages from service");
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();
                if (receivedMessage == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                var messageContent = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Console.WriteLine("Received message: {0}", messageContent);
                switch (messageContent)
                {
                    case "U":
                        Console.Write("Up");
                        _cmdMessenger.SendCommand(new SendCommand(1, int.Parse(ConfigurationManager.AppSettings["power"])));
                        break;
                    case "L":
                        Console.Write("Left");
                        _cmdMessenger.SendCommand(new SendCommand(3));
                        break;
                    case "R":
                        Console.Write("Right");
                        _cmdMessenger.SendCommand(new SendCommand(4));
                        break;
                    case "S":
                        Console.Write("Stop");
                        _cmdMessenger.SendCommand(new SendCommand(5));
                        break;
                    case "H":
                        Console.Write("Headlights");
                        _cmdMessenger.SendCommand(new SendCommand(1, 255));
                        break;
                    default:
                        break;
                }
                
                Console.ResetColor();

                await deviceClient.CompleteAsync(receivedMessage);
            }
        }
    }
}

using System;
using System.Threading;
using WebSocketSharp;

namespace WebSocketClient
{
    public class Program
    {
        const string SERVER = "LOCALHOST";
        static bool ExitProgram = false;
        static WebSocket ws;
        static void Main(string[] args)
        {
            ExitProgram = false;
            OpenConnection();
            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                if (ws != null && ws.IsAlive)
                {
                    Console.WriteLine("Ending Application.");
                    ExitProgram = true;
                    ws.Close();
                    Environment.Exit(0);
                }
            };
        }

        private static void ReceiveMessage()
        {
            var message = string.Empty;
            ws.OnMessage += (sender, e) =>
            {
                message = e.Data;
                Console.WriteLine("Server Message: " + e.Data);
            };
            while (ws.IsAlive)
            {

            }

            ws = null;
            if(!ExitProgram)
                OpenConnection();
        }

        private static void OpenConnection()
        {
            try
            {
                if (ws == null)
                    ws = new WebSocket($"ws://{SERVER}");

                if (!ws.IsAlive)
                {
                    Console.WriteLine("Preparing for start a new connection.");
                    ws.WaitTime = TimeSpan.FromSeconds(3);
                    ws.Connect();
                    if (ws.IsAlive)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Connected");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        if (ws != null && ws.IsAlive)
                            ReceiveMessage();
                    }
                    else
                    {
                        Console.WriteLine("Starting reconnection attempt in 3 seconds.");
                        Thread.Sleep(3000);
                        OpenConnection();
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error on try connect:: {ex.Message}");
                if (ws != null && ws.IsAlive)
                    ws.Close();

                Console.WriteLine("Starting reconnection attempt in 3 seconds.");
                Thread.Sleep(3000);
                OpenConnection();
            }
        }
    }
}

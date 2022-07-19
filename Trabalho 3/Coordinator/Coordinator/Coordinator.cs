using System.Net.Sockets;

namespace Coordinator {
    public static class Coordinator {
        public static object Lock = new();
        public static TcpListener Listener = new(System.Net.IPAddress.Any, 3000);
        public static List<string> Messages = new();
        public static Queue<string> Queue = new();
        public static Dictionary<int, int> ClientsState = new();
        public static void Listen() {
            Console.WriteLine("Waiting for connection...");
            Listener.Start();

            while (true) {
                Thread.Sleep(10);
                var client = Listener.AcceptTcpClient();
                Console.WriteLine("Accepted Client");

                var thread = new Thread(new ParameterizedThreadStart(ClientHandler)) {
                    IsBackground = true
                };
                thread.Start(client);
            }
        }

        public static void ClientHandler(object c) {
            var client = (TcpClient)c;
            var netStream = client.GetStream();

            var connected = true;
            while (connected) {
                Thread.Sleep(10);
                try {
                    var bytes = new byte[client.ReceiveBufferSize];
                    var requestMessage = netStream.Read(bytes, 0, bytes.Length);
                    Console.WriteLine("got data");
                    netStream.Write(bytes, 0, bytes.Length);

                } catch (Exception e) {
                    connected = false;
                    Console.WriteLine(e);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public static void WriteLog(string clientId, string messageType) {
            var folderName = Path.Combine("C:/Users/andre/ProjetosUFRJ/distributed-systems/Trabalho 3/Trabalho3/", "Resultados");
            Directory.CreateDirectory(folderName);
            var fileName = Path.Combine(folderName, "coordinator-logs.txt");
            if (!File.Exists(fileName)) {
                using var fs = File.Create(fileName);
            }
            using var writer = new StreamWriter(fileName, true);
            writer.WriteLine($"Client Id: {clientId}. Message: {messageType}. DateTime: {DateTime.Now}");
        }

        public static void Terminal() {
            while (true) {
                Console.Write("Enter command 1, 2 or 3: ");
                var n = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("");

                switch (n) {
                    case 1:
                        PrintCurrentQueue();
                        break;
                    case 2:
                        PrintClientsState();
                        break;
                    case 3:
                        Console.Write("Finishing execution...");
                        Environment.Exit(Environment.ExitCode);
                        return;
                }
            }
        }
        private static void PrintClientsState() {
            lock (Lock) {
                foreach (var kvp in ClientsState) {
                    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
            }
        }
        private static void PrintCurrentQueue() {
            lock (Lock) {
                foreach (var client in Queue) {
                    Console.WriteLine($"{client}");
                }
            }
        }

        public void UpdateSharedDataSafe() {
            lock (Lock) {

            }
        }
    }
}

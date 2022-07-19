using System.Net.Sockets;
using System.Text;

namespace Coordinator {
    public static class Coordinator {
        public static object Lock = new();
        public static TcpListener Listener = new(System.Net.IPAddress.Any, 3000);
        public static List<string> Messages = new();
        public static Queue<string> Queue = new();
        public static Dictionary<string, int> ClientsState = new();
        public static void Listen() {
            Console.WriteLine("Waiting for connection...");
            Listener.Start();

            while (true) {
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
            var stream = client.GetStream();
            var sr = new StreamReader(stream);
            var sw = new StreamWriter(stream);
            var connected = true;
            while (connected) {
                try {
                    var buffer = new byte[10];
                    stream.Read(buffer, 0, buffer.Length);
                    var message = Encoding.UTF8.GetString(buffer, 0, 10);
                    var clientId = message.Split("|").ToList()[1];
                    UpdateMessagesSafe(message);
                    // if request
                    if (message.StartsWith("1")) {
                        if (!Queue.Any()) {
                            var grant = GenerateMessage(MessageType.Grant, clientId);
                            sw.WriteLine(grant);
                            sw.Flush();
                            UpdateMessagesSafe(message);
                        }
                        AddToQueueSafe(clientId);
                        if (Queue.Any()) {
                            while (Queue.First() != clientId) {
                            }
                            var grant = GenerateMessage(MessageType.Grant, clientId);
                            sw.WriteLine(grant);
                            sw.Flush();
                            UpdateMessagesSafe(message);
                        }
                    }
                    if (!message.StartsWith("3")) {
                        continue;
                    }
                    // if release
                    UpdateClientsStateSafe(Queue.Dequeue());
                    UpdateMessagesSafe(message);

                } catch (Exception e) {
                    connected = false;
                    Console.WriteLine(e);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public static void SendGrantMessage(string clientId) {
            var grant = GenerateMessage(MessageType.Grant, clientId);

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

        public static void UpdateMessagesSafe(string message) {
            lock (Lock) {
                Messages.Add(message);
            }
        }
        public static void UpdateClientsStateSafe(string clientId) {
            lock (Lock) {
                if (ClientsState.TryGetValue(clientId, out _)) {
                    ClientsState[clientId] += 1;
                } else {
                    ClientsState.Add(clientId, 1);
                }
            }
        }
        public static void AddToQueueSafe(string clientId) {
            lock (Lock) {
                Queue.Enqueue(clientId);
            }
        }

        public static string GenerateMessage(MessageType msgType, string clientId) {
            var size = 10;
            var message = $"{Convert.ToInt32(msgType)}|{clientId}|";
            if (message.Length >= size) {
                throw new Exception("Message length was greater than expected!");
            }
            var difference = -message.Length;
            var zeros = new string('0', difference);
            return message + zeros;
        }
        public enum MessageType {
            Unset,
            Request,
            Grant,
            Release
        }
    }
}

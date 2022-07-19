using System.Net.Sockets;
using System.Text;

namespace Coordinator {
    public static class Coordinator {
        public static object Lock = new();
        public static object LogLocker = new();
        public static TcpListener Listener = new(System.Net.IPAddress.Any, 3000);
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
                    var buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                    var message = Encoding.UTF8.GetString(buffer, 0, 10);
                    var clientId = message.Split("|").ToList()[1];
                    // if request
                    if (message.StartsWith("1")) {
                        AddToQueueSafe(clientId);
                        WriteLogSafe(clientId, MessageType.Request);
                        while (!CheckQueueHeadSafe(clientId)) { // spin wait
                        }
                        var grant = GenerateMessage(MessageType.Grant, clientId);
                        sw.WriteLine(grant);
                        WriteLogSafe(clientId, MessageType.Grant);
                        sw.Flush();
                        continue;
                    }
                    // if release
                    string clientToUpdate;
                    lock (Lock) {
                        clientToUpdate = Queue.Dequeue();
                    }
                    UpdateClientsStateSafe(clientToUpdate);
                    WriteLogSafe(clientId, MessageType.Release);
                    return;
                } catch (Exception e) {
                    connected = false;
                    Console.WriteLine(e);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        public static bool CheckQueueHeadSafe(string clientId) {
            lock (Lock) {
                return Queue.First() == clientId;
            }
        }
        public static void WriteLogSafe(string clientId, MessageType msgType) {
            lock (LogLocker) {
                WriteLogUnsafe(clientId, msgType);
            }
        }
        public static void WriteLogUnsafe(string clientId, MessageType msgType) {
            var folderName = Path.Combine("C:/Users/andre/ProjetosUFRJ/distributed-systems/Trabalho 3/", "Resultados");
            Directory.CreateDirectory(folderName);
            var fileName = Path.Combine(folderName, "log.txt");
            if (!File.Exists(fileName)) {
                using var fs = File.Create(fileName);
            }
            var msg = msgType switch {
                MessageType.Request => $"[R] Request - {clientId} - DateTime: {DateTime.Now}",
                MessageType.Grant => $"[G] Grant - {clientId} - DateTime: {DateTime.Now}",
                MessageType.Release => $"[R] Release - {clientId} - DateTime: {DateTime.Now}",
                _ => ""
            };
            using var writer = new StreamWriter(fileName, true);
            writer.WriteLine(msg);
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
            var difference = size - message.Length;
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

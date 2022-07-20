using System.Net.Sockets;
using System.Text;

namespace Coordinator {
    public static class Coordinator {
        public static Dictionary<string, Thread> Threads = new();
        public static object Lock = new();
        public static object LogLocker = new();
        public static Queue<string> Queue = new();
        public static Dictionary<string, int> ClientsState = new();
        public static TcpListener Listener = new(System.Net.IPAddress.Any, 3000);
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
                    Thread.CurrentThread.Name = clientId;
                    // if request
                    if (message.StartsWith("1")) {
                        AddToQueueSafe(clientId);
                        lock (Threads) {
                            Threads.Add(clientId, Thread.CurrentThread);
                        }
                        while (!CheckQueuePeekSafe(clientId)) { // spin wait
                            try {
                                Thread.Sleep(Timeout.Infinite);
                            } catch (ThreadInterruptedException) { }
                        }
                        var grant = GenerateMessage(MessageType.Grant, clientId);
                        WriteLogSafe(clientId, MessageType.Grant);
                        sw.WriteLine(grant);
                        sw.Flush();
                        continue;
                    }
                    // if release
                    WriteLogSafe(clientId, MessageType.Release);
                    lock (Threads) {
                        Threads.Remove(clientId);
                    }
                    sw.WriteLine("ACK");
                    sw.Flush();
                    string clientToUpdate;
                    lock (Lock) {
                        clientToUpdate = Queue.Dequeue();
                        if (Queue.Any()) {
                            Threads[Queue.Peek()].Interrupt();
                        }
                    }
                    UpdateClientsStateSafe(clientToUpdate);
                    return;
                } catch (Exception e) {
                    connected = false;
                    Console.WriteLine(e);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        public static bool CheckQueuePeekSafe(string clientId) {
            lock (Lock) {
                return Queue.Peek() == clientId;
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
                MessageType.Request => $"[R] Request - {clientId} - DateTime: {DateTime.Now}:{DateTime.Now.Millisecond}",
                MessageType.Grant => $"[S] Grant - {clientId} - DateTime: {DateTime.Now}:{DateTime.Now.Millisecond}",
                MessageType.Release => $"[R] Release - {clientId} - DateTime: {DateTime.Now}:{DateTime.Now.Millisecond}",
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
                    Console.WriteLine($"Client = {kvp.Key}, Processed Messages = {kvp.Value}");
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
                WriteLogUnsafe(clientId, MessageType.Request);
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
            Release,
            Ack
        }
    }
}

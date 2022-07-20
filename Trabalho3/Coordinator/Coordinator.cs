using System.Collections;
using System.Net.Sockets;
using System.Text;

public static class Coordinator {
    public static object Lock = new();
    public static Queue<KeyValuePair<string, TcpClient>> Queue = new();
    public static TcpListener Socket = new(System.Net.IPAddress.Any, 3000);
    public static Dictionary<string, int> Granted = new();

    public enum MessageType {
        Unset,
        Request = 1,
        Grant = 2,
        Release = 3
    }

    private static void WriteLog(string clientId, MessageType msgType) {
        var folderName = Path.Combine("C:/Users/andre/ProjetosUFRJ/distributed-systems/Trabalho3/", "Resultados");
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
        var writer = new StreamWriter(fileName, true);
        writer.WriteLine(msg);
        writer.Close();
    }

    private static void SendGrantMessage(string clientId, TcpClient client) {
        var size = 10;
        var message = $"2|{clientId}|";
        if (message.Length >= size) {
            throw new Exception("Message length was greater than expected!");
        }
        var difference = size - message.Length;
        var zeros = new string('0', difference);
        message = message + zeros;

        var stream = client.GetStream();
        var sw = new StreamWriter(stream);
        sw.WriteLine(message);
        sw.Flush();
    }

    private static void Terminal() {
        Console.WriteLine(" ------------------------------------------");
        Console.WriteLine("| Insira:                                  |");
        Console.WriteLine("|   1: imprimir a fila atual               |");
        Console.WriteLine("|   2: para imprimir o estado dos clientes |");
        Console.WriteLine("|   3: para sair                           |");
        Console.WriteLine(" ------------------------------------------");
        while (true) {
            Console.WriteLine("\n-----------------------------------------\n");
            Console.Write("Insira o comando: ");
            var n = Convert.ToInt32(Console.ReadLine());
            switch (n) {
                case 1:
                    var currentQueue = Queue;
                    PrintCurrentQueue(currentQueue);
                    break;
                case 2:
                    var currentGranted = Granted;
                    PrintGranted(currentGranted);
                    break;
                case 3:
                    Environment.Exit(Environment.ExitCode);
                    return;
            }
        }
    }

    private static void Request(string clientId, TcpClient client) {
        lock (Lock) {
            WriteLog(clientId, MessageType.Request);
            if (Queue.Count == 0) {
                Grant(clientId, client);
            }
            Queue.Enqueue(new KeyValuePair<string, TcpClient>(clientId, client));
        }
    }

    private static void PrintCurrentQueue(Queue<KeyValuePair<string, TcpClient>> queue) {
        lock (Lock) {
            Console.WriteLine("");
            Console.Write("<--- [");
            foreach (var kvp in queue) {
                Console.Write($"{kvp.Key}, ");
            }
            Console.Write("] <---");
            Console.WriteLine("");
        }
    }

    private static void Grant(string clientId, TcpClient client) {
        SendGrantMessage(clientId, client);
        WriteLog(clientId, MessageType.Grant);

        if (Granted.TryGetValue(clientId, out _)) {
            Granted[clientId] += 1;
        } else {
            Granted.Add(clientId, 1);
        }
    }

    private static void PrintGranted(Dictionary<string, int> granted) {
        foreach (var kvp in granted) {
            Console.WriteLine("Thread ID = {0}, Granted = {1}", kvp.Key, kvp.Value);
        }
    }

    private static void Release(string clientId) {
        lock (Lock) {
            WriteLog(clientId, MessageType.Release);
            Queue.Dequeue();

            if (Queue.Count == 0) {
                return;
            }
            var (key, value) = Queue.Peek();
            Grant(key, value);
        }
    }

    public static void ClientHandler(object c) {
        var client = (TcpClient)c;
        var stream = client.GetStream();
        var connected = true;
        while (connected) {
            try {
                var buffer = new byte[1024];
                stream.Read(buffer, 0, buffer.Length);
                var message = Encoding.UTF8.GetString(buffer, 0, 10);
                var messageType = message.Split("|").ToList()[0];
                var clientId = message.Split("|").ToList()[1];

                switch (messageType) {
                    case "1":
                        Request(clientId, client);
                        break;
                    case "3":
                        Release(clientId);
                        break;
                }
            } catch (Exception) {
                connected = false;
            }
        }
    }

    public static void Listener() {
        Console.WriteLine("Waiting for connection...");
        Socket.Start();

        while (true) {
            var client = Socket.AcceptTcpClient();
            Console.WriteLine("Accepted Client");

            var thread = new Thread(new ParameterizedThreadStart(ClientHandler)) {
                IsBackground = true
            };
            thread.Start(client);
        }
    }


    private static void PrintCurrentQueue() {
        lock (Lock) {
            Console.WriteLine("");
            foreach (var clientId in Queue) {
                Console.Write($"{clientId}");
            }
            Console.WriteLine("");
        }
    }

    public static void Main() {
        new Thread(Listener).Start();
        Terminal();
    }
}

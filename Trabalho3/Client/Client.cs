using System.Net.Sockets;
using System.Text;

namespace Client {
    public class Client {
        public Client(int id, int repetitions, int waitTime) {
            Id = id;
            Repetitions = repetitions;
            WaitTime = waitTime * 1000;
            Port = 8080;
        }
        public int Id { get; set; }
        public int Repetitions { get; set; }
        public int WaitTime { get; set; }
        public int Port { get; set; }
        public TcpClient SocketClient { get; set; }
        public byte[] SendRequestData => Encoding.ASCII.GetBytes(GenerateMessage(MessageType.Request));
        public byte[] SendReleaseData => Encoding.ASCII.GetBytes(GenerateMessage(MessageType.Release));

        public void Connect() {
            var i = 1;
            var numTries = 0;
        connection:
            if (numTries > 10) {
                return;
            }
            try {
                while (i <= Repetitions) {
                    SocketClient = new TcpClient("127.0.0.1", Port);
                    var stream = SocketClient.GetStream();
                    stream.Write(SendRequestData, 0, SendRequestData.Length);
                    Console.WriteLine($"Client {Id} sending request message to coordinator...");

                    var sr = new StreamReader(stream);
                    var response = sr.ReadLine();
                    if (response.StartsWith("2")) {
                        WriteLog();
                        Console.WriteLine(response);
                        Thread.Sleep(WaitTime);
                        stream.Write(SendReleaseData, 0, SendReleaseData.Length);
                        Console.WriteLine($"Client {Id} sending release message to coordinator...");
                    }
                    i++;
                    stream.Close();
                    SocketClient.Close();
                }
            } catch (Exception) {
                Console.WriteLine("Failed to connect");
                numTries++;
                Thread.Sleep(2000);
                goto connection;
            }
        }

        public void WriteLog() {
            var folderName = Path.Combine($"{Directory.GetCurrentDirectory()}/..", "Resultados");
            Directory.CreateDirectory(folderName);
            var fileName = Path.Combine(folderName, "resultado.txt");
            if (!File.Exists(fileName)) {
                using var fs = File.Create(fileName);
            }
            using var writer = new StreamWriter(fileName, true);
            writer.WriteLine($"Client Id: {Id}. DateTime: {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
        }
        public string GenerateMessage(MessageType msgType) {
            var size = 10;
            var message = $"{Convert.ToInt32(msgType)}|{Id}|";
            if (message.Length >= size) {
                throw new Exception("Message length was greater than expected!");
            }
            var difference = size - message.Length;
            var zeros = new string('0', difference);
            return message + zeros;
        }
    }
    public enum MessageType {
        Unset,
        Request,
        Grant,
        Release
    }
}
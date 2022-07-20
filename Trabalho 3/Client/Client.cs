using System.Net.Sockets;
using System.Text;

namespace Client {
    public class Client {
        public Client(int id, int repetitions, int waitTime) {
            Id = id;
            Repetitions = repetitions;
            WaitTime = waitTime * 1000;
            Port = 3000;
        }
        public int Id { get; set; }
        public int Repetitions { get; set; }
        public int WaitTime { get; set; }
        public int Port { get; set; }
        public TcpClient SocketClient { get; set; }

        public void Connect() {
            var i = 1;
            var numTries = 0;
            var sendRequestData = Encoding.ASCII.GetBytes(GenerateMessage(MessageType.Request));
            var sendReleaseData = Encoding.ASCII.GetBytes(GenerateMessage(MessageType.Release));
        connection:
            if (numTries > 10) {
                return;
            }
            try {
                while (i <= Repetitions) {
                    SocketClient = new TcpClient("127.0.0.1", Port);
                    var stream = SocketClient.GetStream();
                    //request
                    stream.Write(sendRequestData, 0, sendRequestData.Length);
                    Console.WriteLine($"Client {Id} sending request message to coordinator...");

                    //Read Grant
                    var sr = new StreamReader(stream);
                    var response = sr.ReadLine();
                    if (response.StartsWith("2")) {
                        //Enter critical area
                        WriteLog();
                    }
                    Console.WriteLine(response);
                    Thread.Sleep(WaitTime);

                    //release
                    stream.Write(sendReleaseData, 0, sendReleaseData.Length);
                    Console.WriteLine($"Client {Id} sending release message to coordinator...");

                    //Read ACK
                    sr.ReadLine();

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
            var folderName = Path.Combine("C:/Users/andre/ProjetosUFRJ/distributed-systems/Trabalho 3/", "Resultados");
            Directory.CreateDirectory(folderName);
            var fileName = Path.Combine(folderName, "resultado.txt");
            if (!File.Exists(fileName)) {
                using var fs = File.Create(fileName);
            }
            using var writer = new StreamWriter(fileName, true);
            writer.WriteLine($"Client Id: {Id}. DateTime: {DateTime.Now}:{DateTime.Now.Millisecond}");
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

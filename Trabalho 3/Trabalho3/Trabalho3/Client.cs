using System.Net.Sockets;
using System.Text;

namespace Client {
    public class Client {
        private readonly Random _random = new((int)DateTime.Now.Ticks);
        public Client(int id, int repetitions, int waitTime) {
            Id = id;
            Repetitions = repetitions;
            WaitTime = waitTime * 1000;
            Port = _random.Next(1000, 8000);
            SocketClient = new TcpClient("127.0.0.1", Port);
        }
        public int Id { get; set; }
        public int Repetitions { get; set; }
        public int WaitTime { get; set; }
        public int Port { get; set; }
        public TcpClient SocketClient { get; set; }


        public void Connect() {
            var i = 1;
            var numTries = 0;
            var sendRequestData = Encoding.ASCII.GetBytes($"1|{Id}|000000");
            var sendReleaseData = Encoding.ASCII.GetBytes($"3|{Id}|000000");
            NetworkStream stream;
        connection:
            if (numTries > 10) {
                return;
            }
            try {
                stream = SocketClient.GetStream();
            } catch (Exception) {
                Console.WriteLine("Failed to connect");
                numTries++;
                Thread.Sleep(2000);
                goto connection;
            }

            while (i <= Repetitions) {
                //request
                stream.Write(sendRequestData, 0, sendRequestData.Length);
                Console.WriteLine($"Client {Id} sending request message to coordinator...");

                //Read Grant
                var sr = new StreamReader(stream);
                var response = sr.ReadLine();
                if (response?[..1] == "2") {
                    //Enter critical area
                    WriteLog();
                }
                Console.WriteLine(response);
                Thread.Sleep(WaitTime);

                //release
                stream.Write(sendReleaseData, 0, sendReleaseData.Length);
                Console.WriteLine($"Client {Id} sending release message to coordinator...");

                i++;
            }
        }

        public void WriteLog() {
            var folderName = Path.Combine("C:/Users/andre/ProjetosUFRJ/distributed-systems/Trabalho 3/Trabalho3/", "Resultados");
            Directory.CreateDirectory(folderName);
            var fileName = Path.Combine(folderName, "resultado.txt");
            if (!File.Exists(fileName)) {
                using var fs = File.Create(fileName);
            }
            using var writer = new StreamWriter(fileName, true);
            writer.WriteLine($"Client Id: {Id}. DateTime: {DateTime.Now}");
        }

    }
}

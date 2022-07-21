using System.Net.Sockets;
using System.Text;

namespace Client
{
  public class Client
  {
    public Client(int id, int repetitions, int waitTime)
    {
      Id = id;
      Repetitions = repetitions;
      WaitTime = waitTime * 1000;
      Port = 8080;
    }
    #region props
    public int Id { get; set; }
    public int Repetitions { get; set; }
    public int WaitTime { get; set; }
    public int Port { get; set; }
    public TcpClient SocketClient { get; set; }
    #endregion

    #region Connect
    private void ListenGrant()
    {
      var stream = SocketClient.GetStream();
      var sr = new StreamReader(stream);
      var response = sr.ReadLine();
      if (response.StartsWith("2"))
      {
        WriteLog();
        Console.WriteLine($" > Coordinator granted access to client {Id}");
        Thread.Sleep(WaitTime);

        sendMessage(MessageType.Release);
        Console.WriteLine($" > Client {Id} sent release message to coordinator");
      }
    }

    public void Connect()
    {
      var i = 1;
      var numTries = 0;
    tryConnection:
      if (numTries > 10)
      {
        return;
      }
      try
      {
        while (i <= Repetitions)
        {
          SocketClient = new TcpClient("127.0.0.1", Port);

          sendMessage(MessageType.Request);
          Console.WriteLine($" > Client {Id} sent request message to coordinator");

          ListenGrant();

          i++;
          SocketClient.Close();
        }
      }
      catch (Exception)
      {
        Console.WriteLine("Failed to connect");
        numTries++;
        Thread.Sleep(2000);
        goto tryConnection;
      }
    }
    #endregion

    #region Auxiliary methods
    public void WriteLog()
    {
      var folderName = Path.Combine($"{Directory.GetCurrentDirectory()}/..", "Resultados");

      Directory.CreateDirectory(folderName);
      var fileName = Path.Combine(folderName, "resultado.txt");
      if (!File.Exists(fileName))
      {
        using var fs = File.Create(fileName);
      }
      using var writer = new StreamWriter(fileName, true);
      writer.WriteLine($"Client Id: {Id}. DateTime: {DateTime.Now:dd/MM/yyyy HH:mm:ss.fff}");
    }

    public void sendMessage(MessageType msgType)
    {
      var stream = SocketClient.GetStream();
      var size = 10;
      var message = $"{Convert.ToInt32(msgType)}|{Id}|";
      if (message.Length >= size)
      {
        throw new Exception("Message length was greater than expected!");
      }
      var difference = size - message.Length;
      var zeros = new string('0', difference);
      message = message + zeros;
      var data = Encoding.ASCII.GetBytes(message);
      stream.Write(data, 0, data.Length);
    }
    #endregion
  }

  public enum MessageType
  {
    Request = 1,
    Grant = 2,
    Release = 3
  }
}
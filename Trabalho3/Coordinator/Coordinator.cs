using System.Net.Sockets;
using System.Text;


public static class Coordinator
{
  public static object Lock = new();
  public static Queue<KeyValuePair<string, TcpClient>> Queue = new();
  public static TcpListener socket = new(System.Net.IPAddress.Any, 3000);
  public static Dictionary<string, int> Granted = new();


  public enum MessageType
  {
    Request = 1,
    Grant = 2,
    Release = 3
  }

  private static void writeLog(string clientId, MessageType msgType)
  {
    var folderName = Path.Combine("../", "Resultados");
    Directory.CreateDirectory(folderName);
    var fileName = Path.Combine(folderName, "log.txt");
    if (!File.Exists(fileName))
    {
      using var fs = File.Create(fileName);
    }
    var msg = msgType switch
    {
      MessageType.Request => $"[R] Request - {clientId} - DateTime: {DateTime.Now}:{DateTime.Now.Millisecond}",
      MessageType.Grant => $"[S] Grant - {clientId} - DateTime: {DateTime.Now}:{DateTime.Now.Millisecond}",
      MessageType.Release => $"[R] Release - {clientId} - DateTime: {DateTime.Now}:{DateTime.Now.Millisecond}",
      _ => ""
    };
    var writer = new StreamWriter(fileName, true);
    writer.WriteLine(msg);
    writer.Close();
  }

  private static void sendGrantMessage(string clientId, TcpClient client)
  {
    var size = 10;
    var message = $"2|{clientId}|";
    if (message.Length >= size)
    {
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

  private static void request(string clientId, TcpClient client)
  {
    lock (Lock)
    {
      writeLog(clientId, MessageType.Request);
      if (Queue.Count == 0)
      {
        grant(clientId, client);
      }
      Queue.Enqueue(new KeyValuePair<string, TcpClient>(clientId, client));
    }
  }

  private static void grant(string clientId, TcpClient client)
  {
    sendGrantMessage(clientId, client);
    writeLog(clientId, MessageType.Grant);

    if (Granted.TryGetValue(clientId, out _))
    {
      Granted[clientId] += 1;
    }
    else
    {
      Granted.Add(clientId, 1);
    }
  }

  private static void release(string clientId)
  {
    lock (Lock)
    {
      writeLog(clientId, MessageType.Release);
      Queue.Dequeue();

      if (Queue.Count != 0)
      {
        var kvp = Queue.Peek();
        grant(kvp.Key, kvp.Value);
      }
    }
  }

  public static void clientHandler(object c)
  {
    var client = (TcpClient)c;
    var stream = client.GetStream();
    var sw = new StreamWriter(stream);
    var connected = true;
    while (connected)
    {
      try
      {
        var buffer = new byte[1024];
        stream.Read(buffer, 0, buffer.Length);
        var message = Encoding.UTF8.GetString(buffer, 0, 10);
        var messageType = message.Split("|").ToList()[0];
        var clientId = message.Split("|").ToList()[1];
        switch (messageType)
        {
          case "1":
            request(clientId, client);
            break;
          case "3":
            release(clientId);
            break;
          default:
            break;
        }
      }
      catch (Exception e)
      {
        connected = false;
        Console.WriteLine(e);
        Console.WriteLine(e.StackTrace);
      }
    }
  }

  public static void listener()
  {
    Console.WriteLine("Waiting for connection...");
    socket.Start();

    while (true)
    {
      var client = socket.AcceptTcpClient();
      Console.WriteLine("Accepted Client");

      var thread = new Thread(new ParameterizedThreadStart(clientHandler))
      {
        IsBackground = true
      };
      thread.Start(client);
    }
  }


  private static void terminal()
  {
    Console.WriteLine("Insira:");
    Console.WriteLine("  1: imprimir a fila atual");
    Console.WriteLine("  2: para imprimir o estado dos clientes");
    Console.WriteLine("  3: para sair");
    while (true)
    {
      Console.Write("Insira o comando: ");
      var n = Convert.ToInt32(Console.ReadLine());
      Console.WriteLine("");
      switch (n)
      {
        case 1:
          printCurrentQueue();
          break;
        case 2:
          // PrintClientsState();
          break;
        case 3:
          Environment.Exit(Environment.ExitCode);
          return;
      }
    }
  }

  private static void printCurrentQueue()
  {
    lock (Lock)
    {
      Console.WriteLine("");
      foreach (var clientId in Queue)
      {
        Console.Write($"{clientId}");
      }
      Console.WriteLine("");
    }
  }

  public static void Main()
  {
    new Thread(listener).Start();
    terminal();
  }
}

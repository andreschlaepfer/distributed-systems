namespace Coordinator;
public class Run {
    public static void Main() {
        var listeningThread = new Thread(Coordinator.Listener) {
            IsBackground = true
        };
        listeningThread.Start();
        Coordinator.Terminal();
    }
}
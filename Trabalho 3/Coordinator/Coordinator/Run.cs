namespace Coordinator {
    public class Run {
        public static void Main() {
            var listeningThread = new Thread(Coordinator.Listen) {
                IsBackground = true
            };
            listeningThread.Start();
            Coordinator.Terminal();
        }
    }
}

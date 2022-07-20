namespace Client {
    public class Run {
        public static void Main() {
            Console.WriteLine("Enter N:");
            var n = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter R:");
            var r = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Enter K:");
            var k = Convert.ToInt32(Console.ReadLine());

            for (var i = 1; i <= n; i++) {
                var newClient = new Client(i, r, k);
                var newThread = new Thread(newClient.Connect);
                newThread.Start();
            }
        }
    }
}

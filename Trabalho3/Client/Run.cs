namespace Client {
    public class Run {
        public static void Main(string[] args) {
            if (args.Length != 3) {
                Console.WriteLine("\nErro:\n  Insira os argumentos corretamente: <n> <r> <k>");
                return;
            }
            // arguments must be numbers
            if (!int.TryParse(args[0], out int n) || !int.TryParse(args[1], out int r) || !int.TryParse(args[2], out int k)) {
                Console.WriteLine("\nErro:\n  Argumentos inválidos");
                return;
            }



            n = Convert.ToInt32(args[0]);
            r = Convert.ToInt32(args[1]);
            k = Convert.ToInt32(args[2]);

            for (var i = 1; i <= n; i++) {
                var newClient = new Client(i, r, k);
                var newThread = new Thread(newClient.Connect);
                newThread.Start();
            }
        }
    }
}
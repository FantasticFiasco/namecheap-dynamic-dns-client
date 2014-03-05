namespace DynDnsClient.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Print("Updating Namecheap host records...");
            
            var client = new Client();
            client.RunOnce();

            Print("Press any key to continue...");
            ReadKey();
        }

        private static void Print(string message)
        {
            System.Console.WriteLine(message);
        }

        private static void ReadKey()
        {
            System.Console.ReadKey();
        }
    }
}
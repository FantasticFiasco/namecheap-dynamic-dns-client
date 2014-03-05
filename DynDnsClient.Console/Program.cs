using System.Reflection;
using log4net;

namespace DynDnsClient.Console
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Log.Info("Updating Namecheap host records...");
            
            var client = new Client();
            client.RunOnce();

            Log.Info("Press any key to continue...");
            System.Console.ReadKey();
        }
    }
}
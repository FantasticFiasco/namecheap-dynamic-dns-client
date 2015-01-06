using System;
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

            try
            {
                using (var client = new Client())
                {
                    client.RunOnce();
                }
            }
            catch (Exception e)
            {
                Log.Fatal("Unable to run client.", e);
            }
            
            Log.Info("Press any key to continue...");
            System.Console.ReadKey();
        }
    }
}
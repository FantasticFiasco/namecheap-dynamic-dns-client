using System.Reflection;
using System.ServiceProcess;
using log4net;

namespace DynDnsClient.Service
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Log.Info("Starting service");

            var servicesToRun = new ServiceBase[] 
            { 
                new Service() 
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
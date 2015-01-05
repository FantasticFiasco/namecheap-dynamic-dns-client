using System.Reflection;
using System.ServiceProcess;
using DynDnsClient.Properties;
using log4net;

namespace DynDnsClient.Service
{
    public partial class Service : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Client client;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Starting service");

            client = new Client();
            client.RunContinuously();
        }

        protected override void OnStop()
        {
            Log.Info("Stopping service");

            client.Dispose();
            
            Settings.Default.Save();
        }
    }
}
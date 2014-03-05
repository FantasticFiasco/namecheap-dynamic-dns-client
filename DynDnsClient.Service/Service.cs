using System.Reflection;
using System.ServiceProcess;
using log4net;

namespace DynDnsClient.Service
{
    public partial class Service : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Client client;

        public Service()
        {
            InitializeComponent();

            client = new Client();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Starting service");

            client.RunContinuously();
        }

        protected override void OnStop()
        {
            Log.Info("Stopping service");

            client.Stop();
        }
    }
}
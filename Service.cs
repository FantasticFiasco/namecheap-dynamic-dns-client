using System.Reflection;
using System.ServiceProcess;
using DynDnsClient.Properties;
using log4net;

namespace DynDnsClient
{
    public partial class Service : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Info("Starting service");
        }

        protected override void OnStop()
        {
            Log.Info("Stopping service");

            Settings.Default.Save();
        }
    }
}
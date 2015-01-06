using System;
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
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Log.Info("Starting service");

            try
            {
                client = new Client();
                client.RunContinuously();
            }
            catch (Exception e)
            {
                Log.Error("Unable to run the client continuously.", e);
                throw;
            }
        }

        protected override void OnStop()
        {
            Log.Info("Stopping service");

            client.Dispose();
            
            Settings.Default.Save();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal("Unhandled exception when running the service.", (Exception)e.ExceptionObject);
        }
    }
}
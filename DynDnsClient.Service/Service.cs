using System;
using System.Reflection;
using System.ServiceProcess;
using log4net;

namespace DynDnsClient.Service
{
    public partial class Service : ServiceBase
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private SavedData savedData;
        private Client client;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            Log.Info("Starting service");

            savedData = SavedData.Load();

            try
            {
                client = new Client(savedData);
                client.Run();
            }
            catch (Exception e)
            {
                Log.Error("Unable to run the client continuously", e);
                throw;
            }
        }

        protected override void OnStop()
        {
            Log.Info("Stopping service");

            client.Dispose();
            
            SavedData.Save(savedData);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal("Unhandled exception when running the service", (Exception)e.ExceptionObject);
        }
    }
}
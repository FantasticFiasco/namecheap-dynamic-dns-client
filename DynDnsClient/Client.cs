using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using DynDnsClient.Properties;
using log4net;

namespace DynDnsClient
{
    public class Client
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ExternalIpAddress externalIpAddress;
        private readonly NamecheapClient namecheapClient;

        private Timer timer;

        public Client()
        {
            externalIpAddress = new ExternalIpAddress();
            namecheapClient = new NamecheapClient();
        }

        public void RunContinuously()
        {
            Log.Info("Running client continuously");

            timer = new Timer(OnTick, null, TimeSpan.Zero, Settings.Default.Period);
        }

        public void Stop()
        {
            Log.Info("Stopping client");

            timer.Dispose();
        }

        private void OnTick(object state)
        {
            // Get external IP
            string ipAddress = externalIpAddress.Get();
            if (ipAddress == null)
            {
                Log.Error("Cannot proceed since external IP address is unknown.");
                return;
            }

            // Determine if external IP has changed
            if (ipAddress == Settings.Default.LastKnownExternalIpAddress)
            {
                Log.Info("External IP has not changed, is still " + ipAddress);
                return;
            }

            Log.InfoFormat(
                "External IP has changed from {0} to {1}, lets update Namecheap records",
                Settings.Default.LastKnownExternalIpAddress,
                ipAddress);

            // Update Namecheap records
            DnsUpdateResult result = namecheapClient.Update(
                Settings.Default.Domain,
                Settings.Default.Hosts.Cast<string>(),
                Settings.Default.Password,
                ipAddress);

            if (result.IsSuccess)
            {
                Log.Info("Successfully updated Namecheap records." + Environment.NewLine + result);
                Settings.Default.LastKnownExternalIpAddress = ipAddress;
            }
            else
            {
                Log.Error("Unable to update Namecheap records" + Environment.NewLine + result);
            }
        }
    }
}
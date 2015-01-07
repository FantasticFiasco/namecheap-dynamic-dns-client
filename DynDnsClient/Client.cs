using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using DynDnsClient.Properties;
using log4net;

namespace DynDnsClient
{
    public class Client : IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly ExternalIpAddress externalIpAddress;
        private readonly NamecheapClient namecheapClient;
        private readonly Hosts hosts;

        private Timer timer;

        public Client()
        {
            externalIpAddress = new ExternalIpAddress();
            namecheapClient = new NamecheapClient();
            
            hosts = new Hosts();
            hosts.Changed += UpdateDueToChangedHosts;
        }

        public void RunContinuously()
        {
            Log.Info("Running client continuously");
            
            timer = new Timer(UpdateDueToTimeout, null, TimeSpan.Zero, Settings.Default.Period);
        }

        public void RunOnce()
        {
            Log.Info("Running client once");

            // Force update by specifying that last known IP address is unknown
            string lastKnownExternalIpAddress = Settings.Default.LastKnownExternalIpAddress;
            Settings.Default.LastKnownExternalIpAddress = null;

            UpdateDueToTimeout(null);

            // Restore last known IP address
            Settings.Default.LastKnownExternalIpAddress = lastKnownExternalIpAddress;
        }

        private void UpdateDueToChangedHosts(object sender, EventArgs e)
        {
            string[] addedHosts = hosts.AddedSinceLastRead();
            
            if (addedHosts.Any())
            {
                Log.InfoFormat("Hosts file changed, {0} hosts where added", addedHosts.Length);
                Update(addedHosts);
            }
            else
            {
                Log.Info("Hosts file changed, no hosts where added");
            }
        }

        private void UpdateDueToTimeout(object state)
        {
            // Get external IP
            string ipAddress = externalIpAddress.Get();
            if (ipAddress == null)
            {
                Log.Error("Cannot proceed since external IP address is unknown");
                return;
            }

            // Determine if external IP has changed
            if (ipAddress == Settings.Default.LastKnownExternalIpAddress)
            {
                Log.InfoFormat("External IP has not changed, is still {0}", ipAddress);
                return;
            }

            Log.InfoFormat(
                "External IP has changed from {0} to {1}, proceed with updating Namecheap records",
                Settings.Default.LastKnownExternalIpAddress,
                ipAddress);

            if (Update(hosts.Read()))
            {
                Settings.Default.LastKnownExternalIpAddress = ipAddress;
            }
        }

        private bool Update(IEnumerable<string> hosts)
        {
            // Update Namecheap records
            DnsUpdateResult result = namecheapClient.Update(
                Settings.Default.Domain,
                hosts,
                Settings.Default.Password);

            if (result.IsSuccess)
            {
                Log.InfoFormat("Successfully updated Namecheap records{0}{1}", Environment.NewLine, result);
            }
            else
            {
                Log.ErrorFormat("Unable to update Namecheap records{0}{1}", Environment.NewLine, result);
            }

            return result.IsSuccess;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (hosts != null)
                {
                    hosts.Dispose();
                }

                if (timer != null)
                {
                    timer.Dispose();
                }
            }
        }

        #endregion
    }
}
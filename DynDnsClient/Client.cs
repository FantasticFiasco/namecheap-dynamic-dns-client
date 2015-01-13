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

        private readonly SavedData savedData;
        private readonly WanAddressResolver wanAddressResolver;
        private readonly DnsUpdater dnsUpdater;
        private readonly Hosts hosts;

        private Timer timer;

        public Client(SavedData savedData)
        {
            this.savedData = savedData;
            wanAddressResolver = new WanAddressResolver();
            dnsUpdater = new DnsUpdater();
            
            hosts = new Hosts();
            hosts.Changed += UpdateDueToChangedHosts;
        }

        public void Run()
        {
            Log.Info("Running client");
            
            timer = new Timer(UpdateDueToTimeout, null, TimeSpan.Zero, Settings.Default.Period);
        }

        private void UpdateDueToChangedHosts(object sender, EventArgs e)
        {
            string[] addedHosts = hosts.Read()
                .Except(savedData.Hosts)
                .ToArray();
            
            if (addedHosts.Any())
            {
                Log.InfoFormat("Hosts file changed, {0} hosts where added", addedHosts.Length);

                if (Update(addedHosts))
                {
                    savedData.Hosts.AddRange(addedHosts);
                }
            }
            else
            {
                Log.Info("Hosts file changed, no hosts where added");
            }
        }

        private void UpdateDueToTimeout(object state)
        {
            // Resolve WAN address
            string address = wanAddressResolver.Resolve();
            if (address == null)
            {
                Log.Error("Cannot proceed since WAN address is unknown");
                return;
            }

            // Determine if WAN address has changed
            if (address == savedData.LastKnownWanAddress)
            {
                Log.InfoFormat("WAN address has not changed, is still {0}", address);
                return;
            }

            Log.InfoFormat(
                "WAN address has changed from {0} to {1}, proceed with updating Namecheap records",
                savedData.LastKnownWanAddress,
                address);

            string[] existingHosts = hosts.Read();

            if (Update(existingHosts))
            {
                savedData.LastKnownWanAddress = address;
                savedData.Hosts = new List<string>(existingHosts);
            }
        }

        private bool Update(IEnumerable<string> hosts)
        {
            // Update Namecheap records
            DnsUpdateResult result = dnsUpdater.Update(
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
using System;
using System.Collections.Generic;
using System.IO;
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
        private static readonly string HostsFileName = "Hosts.txt";

        private readonly ExternalIpAddress externalIpAddress;
        private readonly NamecheapClient namecheapClient;
        private readonly List<string> hosts;

        private Timer timer;

        public Client()
        {
            externalIpAddress = new ExternalIpAddress();
            namecheapClient = new NamecheapClient();
            hosts = LoadHosts();
        }

        public void RunContinuously()
        {
            Log.Info("Running client continuously");
            
            timer = new Timer(Update, null, TimeSpan.Zero, Settings.Default.Period);
        }

        public void RunOnce()
        {
            Log.Info("Running client once");

            // Force update, this will not be saved since Stop isn't called when only running it
            // once
            Settings.Default.LastKnownExternalIpAddress = null;

            Update(null);
        }

        public void Stop()
        {
            Log.Info("Stopping client");
            
            timer.Dispose();

            Settings.Default.Save();
        }

        private void Update(object state)
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
                hosts,
                Settings.Default.Password);

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

        private static List<string> LoadHosts()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string hostsFilePath = Path.Combine(directory, HostsFileName);

            if (!File.Exists(hostsFilePath))
            {
                Log.WarnFormat("No hosts file found at '{0}'", hostsFilePath);
                return new List<string>();
            }

            List<string> hosts = File.ReadAllLines(hostsFilePath)
                .Where(host => !string.IsNullOrWhiteSpace(host))
                .Select(host => host.Trim())
                .ToList();
            
            Log.InfoFormat("Hosts: {0}", string.Join(", ", hosts));

            return hosts;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;

namespace DynDnsClient
{
    internal class Hosts
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string FileName = "Hosts.txt";

        internal List<string> Get()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string hostsFilePath = Path.Combine(directory, FileName);

            if (!File.Exists(hostsFilePath))
            {
                Log.WarnFormat("No hosts file found at '{0}'", hostsFilePath);
                return new List<string>();
            }

            List<string> hosts = File.ReadAllLines(hostsFilePath)
                .Select(host => host.Trim())
                .Where(host => !string.IsNullOrWhiteSpace(host))
                .Where(host => !host.StartsWith("#"))
                .ToList();
            
            Log.InfoFormat("Hosts: {0}", string.Join(", ", hosts));

            return hosts;
        }
    }
}
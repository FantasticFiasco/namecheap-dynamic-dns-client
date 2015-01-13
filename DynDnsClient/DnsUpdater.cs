using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using log4net;

namespace DynDnsClient
{
    internal class DnsUpdater
    {
        private const string Url = "https://dynamicdns.park-your-domain.com/update?host={0}&domain={1}&password={2}";
        
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public DnsUpdateResult Update(string domain, IEnumerable<string> hosts, string password)
        {
            var result = new DnsUpdateResult();

            foreach (string host in hosts)
            {
                string url = CreateUrl(domain, host, password);
                bool success = Request(url);

                result.Add(host, success);
            }

            return result;
        }

        private static bool Request(string url)
        {
            using (var client = new WebClient())
            {
                try
                {
                    string response = client.DownloadString(url);
                    return response.Contains("<ErrCount>0</ErrCount>");
                }
                catch (Exception e)
                {
                    Log.Error("Unable to update records with " + url, e);
                    return false;
                }
            }
        }

        private static string CreateUrl(string domain, string host, string password)
        {
            return string.Format(Url, host, domain, password);
        }
    }
}
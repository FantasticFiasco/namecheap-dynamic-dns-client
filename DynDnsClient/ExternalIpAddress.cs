using System.Net;
using System.Reflection;
using log4net;

namespace DynDnsClient
{
    internal class ExternalIpAddress
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string Url = "http://checkip.dyndns.com/";

        internal string Get()
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    return Parse(webClient.DownloadString(Url));
                }
                catch (WebException e)
                {
                    Log.Error("Unable to get external IP.", e);
                    return null;
                }
            }
        }

        private static string Parse(string response)
        {
            return response
                .Replace(
                    "<html><head><title>Current IP Check</title></head><body>Current IP Address: ",
                    string.Empty)
                .Replace(
                    "</body></html>",
                    string.Empty);
        }
    }
}
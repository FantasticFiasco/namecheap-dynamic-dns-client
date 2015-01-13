using System.Net;
using System.Reflection;
using log4net;

namespace DynDnsClient
{
    internal class WanAddressResolver
    {
        private const string Url = "http://checkip.dyndns.com/";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        internal string Resolve()
        {
            using (var webClient = new WebClient())
            {
                try
                {
                    return Parse(webClient.DownloadString(Url));
                }
                catch (WebException e)
                {
                    Log.Error("Unable to get WAN address", e);
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
                    string.Empty)
                .Trim();
        }
    }
}
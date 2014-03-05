using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynDnsClient
{
    internal class DnsUpdateResult
    {
        private readonly List<PartialResult> partialResults;

        public DnsUpdateResult()
        {
            partialResults = new List<PartialResult>();
        }

        public bool IsSuccess
        {
            get { return partialResults.All(partialResult => partialResult.Success); }
        }

        public void Add(string host, bool success)
        {
            var partialResult = new PartialResult
            {
                Host = host,
                Success = success
            };

            partialResults.Add(partialResult);
        }

        public override string ToString()
        {
            var resultBuilder = new StringBuilder();

            foreach (PartialResult partialResult in partialResults.OrderBy(partialResult => partialResult.Success))
            {
                resultBuilder.AppendFormat("Host: {0}; Success: {1}", partialResult.Host, partialResult.Success);
            }

            return resultBuilder.ToString();
        }

        private class PartialResult
        {
            public string Host { get; set; }
            public bool Success { get; set; }
        }
    }
}
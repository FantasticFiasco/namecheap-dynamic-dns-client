using System;
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
            partialResults.Add(new PartialResult(host, success));
        }

        public override string ToString()
        {
            var resultBuilder = new StringBuilder();

            foreach (PartialResult partialResult in partialResults.OrderBy(partialResult => partialResult.Success))
            {
                resultBuilder.AppendFormat(
                    "{0}: {1}{2}",
                    partialResult.Host,
                    partialResult.Success ? "Success" : "Failure",
                    Environment.NewLine);
            }

            return resultBuilder.ToString();
        }

        private class PartialResult
        {
            private readonly string host;
            private readonly bool success;

            internal PartialResult(string host, bool success)
            {
                this.host = host;
                this.success = success;
            }

            public string Host
            {
                get { return host; }
            }

            public bool Success
            {
                get { return success; }
            }
        }
    }
}
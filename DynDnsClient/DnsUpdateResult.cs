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
            var results = new List<string>();

            foreach (PartialResult partialResult in partialResults.OrderBy(partialResult => partialResult.Success))
            {
                results.Add(string.Format(
                    "{0}: {1}",
                    partialResult.Host,
                    partialResult.Success ? "Success" : "Failure"));
            }

            return string.Join(Environment.NewLine, results);
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
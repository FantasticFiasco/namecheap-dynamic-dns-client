using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;

namespace DynDnsClient
{
    internal class Hosts : IDisposable
    {
        private const string FileName = "Hosts.txt";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private readonly FileSystemWatcher hostsWatcher;

        private EventHandler changedHandler;
        private string[] hosts; 

        internal Hosts()
        {
            hostsWatcher = new FileSystemWatcher(FilePath, FileName);
            hostsWatcher.Created += (sender, e) => FireChanged();
            hostsWatcher.Deleted += (sender, e) => FireChanged();
            hostsWatcher.Changed += (sender, e) => FireChanged();
            hostsWatcher.Renamed += (sender, e) => FireChanged();
        }

        internal event EventHandler Changed
        {
            add { changedHandler += value; }
            remove { changedHandler -= value; }
        }

        internal string[] Read()
        {
            hosts = InternalRead();

            if (hosts.Length == 0)
            {
                Log.WarnFormat("Either file '{0}' wasn't found or no hosts exists in the file.", FilePath);
            }
            else
            {
                Log.InfoFormat("Hosts: {0}", string.Join(", ", hosts));    
            }

            return hosts;
        }

        internal string[] AddedSinceLastRead()
        {
            return InternalRead()
                .Except(hosts)
                .ToArray();
        }

        private static string[] InternalRead()
        {
            if (!File.Exists(FilePath))
            {
                return new string[0];
            }

            return File.ReadAllLines(FilePath)
                .Select(host => host.Trim())
                .Where(host => !string.IsNullOrWhiteSpace(host))
                .Where(host => !host.StartsWith("#"))
                .ToArray();
        }

        private void FireChanged()
        {
            EventHandler handler = changedHandler;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private static string FilePath
        {
            get { return Path.Combine(Directories.CurrentDirectory, FileName); }
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
                if (hostsWatcher != null)
                {
                    hostsWatcher.Dispose();
                }
            }
        }

        #endregion
    }
}
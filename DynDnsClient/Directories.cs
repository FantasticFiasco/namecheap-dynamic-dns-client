using System;
using System.IO;
using System.Reflection;

namespace DynDnsClient
{
    internal static class Directories
    {
        private static readonly Lazy<string> LazyCurrentDirectory;

        static Directories()
        {
            LazyCurrentDirectory = new Lazy<string>(CreateCurrentDirectory);
        }

        internal static string CurrentDirectory
        {
            get { return LazyCurrentDirectory.Value; }
        }

        private static string CreateCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
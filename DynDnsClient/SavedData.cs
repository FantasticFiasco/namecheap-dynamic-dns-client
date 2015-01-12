using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DynDnsClient
{
    public class SavedData
    {
        private const string FileName = "SavedData.xml";

        public SavedData()
        {
            Hosts = new List<string>();
        }

        public string LastKnownExternalIpAddress { get; set; }

        public List<string> Hosts { get; set; }

        public static SavedData Load()
        {
            if (!File.Exists(FilePath))
            {
                return new SavedData();
            }

            using (FileStream reader = File.OpenRead(FilePath))
            {
                var serializer = new XmlSerializer(typeof(SavedData));
                return (SavedData)serializer.Deserialize(reader);
            }
        }

        public static void Save(SavedData data)
        {
            using (FileStream writer = File.Create(FilePath))
            {
                var serializer = new XmlSerializer(typeof(SavedData));
                serializer.Serialize(writer, data);
            }
        }

        private static string FilePath
        {
            get { return Path.Combine(Directories.CurrentDirectory, FileName); }
        }
    }
}
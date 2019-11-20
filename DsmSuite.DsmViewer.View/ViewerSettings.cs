using System;
using System.Xml;
using System.Xml.Serialization;

namespace DsmSuite.DsmViewer.View
{
    /// <summary>
    /// Settings used by dsm builder. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class ViewerSettings
    {
        private bool _loggingEnabled;

        public static ViewerSettings CreateDefault()
        {
            ViewerSettings settings = new ViewerSettings
            {
                LoggingEnabled = false,
            };

            return settings;
        }

        public bool LoggingEnabled
        {
            get { return _loggingEnabled; }
            set { _loggingEnabled = value; }
        }
        public static void WriteToFile(string filename, ViewerSettings builderSettings)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = new XmlSerializer(typeof(ViewerSettings));

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, builderSettings);
            }
        }

        public static ViewerSettings ReadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ViewerSettings));

            ViewerSettings builderSettings;
            using (XmlReader reader = XmlReader.Create(filename))
            {
                builderSettings = (ViewerSettings)serializer.Deserialize(reader);
            }

            return builderSettings;
        }
    }
}


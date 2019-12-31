using System;
using System.Xml;
using System.Xml.Serialization;

namespace DsmSuite.DsmViewer.View.Settings
{
    /// <summary>
    /// Settings used by dsm builder. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class ViewerSettings
    {
        private bool _loggingEnabled;
        private Theme _theme;

        public static ViewerSettings CreateDefault()
        {
            ViewerSettings settings = new ViewerSettings
            {
                LoggingEnabled = false,
                Theme = Theme.Light,
            };

            return settings;
        }

        public bool LoggingEnabled
        {
            get { return _loggingEnabled; }
            set { _loggingEnabled = value; }
        }

        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        public static void WriteToFile(string filename, ViewerSettings viewerSettings)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = new XmlSerializer(typeof(ViewerSettings));

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, viewerSettings);
            }
        }

        public static ViewerSettings ReadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ViewerSettings));

            ViewerSettings viewerSettings;
            using (XmlReader reader = XmlReader.Create(filename))
            {
                viewerSettings = (ViewerSettings)serializer.Deserialize(reader);
            }

            return viewerSettings;
        }
    }
}


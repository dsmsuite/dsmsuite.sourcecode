using System;
using System.Xml;
using System.Xml.Serialization;

namespace DsmSuite.DsmViewer.ViewModel.Settings
{
    /// <summary>
    /// Settings used by dsm builder. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class ViewerSettingsData
    {
        private bool _loggingEnabled;
        private bool _showCycles;
        private Theme _theme;

        public static ViewerSettingsData CreateDefault()
        {
            ViewerSettingsData settings = new ViewerSettingsData
            {
                LoggingEnabled = false,
                ShowCycles = false,
                Theme = Theme.Light,
            };

            return settings;
        }

        public bool LoggingEnabled
        {
            get { return _loggingEnabled; }
            set { _loggingEnabled = value; }
        }

        public bool ShowCycles
        {
            get { return _showCycles; }
            set { _showCycles = value; }
        }

        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        public static void WriteToFile(string filename, ViewerSettingsData viewerSettings)
        {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
            XmlSerializer serializer = new XmlSerializer(typeof(ViewerSettingsData));

            using (XmlWriter xmlWriter = XmlWriter.Create(filename, xmlWriterSettings))
            {
                serializer.Serialize(xmlWriter, viewerSettings);
            }
        }

        public static ViewerSettingsData ReadFromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ViewerSettingsData));

            ViewerSettingsData viewerSettings;
            using (XmlReader reader = XmlReader.Create(filename))
            {
                viewerSettings = (ViewerSettingsData)serializer.Deserialize(reader);
            }

            return viewerSettings;
        }
    }
}


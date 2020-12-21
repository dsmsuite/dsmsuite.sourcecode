using System;
using System.Xml;
using System.Xml.Serialization;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.ViewModel.Settings
{
    /// <summary>
    /// Settings used by dsm builder. Persisted in XML format using serialization.
    /// </summary>
    [Serializable]
    public class ViewerSettingsData
    {
        private LogLevel _logLevel;
        private bool _showCycles;
        private bool _caseSensitiveSearch;
        private bool _betaFeaturesEnabled;
        private Theme _theme;

        public static ViewerSettingsData CreateDefault()
        {
            ViewerSettingsData settings = new ViewerSettingsData
            {
                LogLevel = LogLevel.None,
                ShowCycles = true,
                BetaFeaturesEnabled = false,
                Theme = Theme.Light,
            };

            return settings;
        }

        public LogLevel LogLevel
        {
            get { return _logLevel; }
            set { _logLevel = value; }
        }

        public bool ShowCycles
        {
            get { return _showCycles; }
            set { _showCycles = value; }
        }

        public bool CaseSensitiveSearch
        {
            get { return _caseSensitiveSearch; }
            set { _caseSensitiveSearch = value; }
        }       

        public bool BetaFeaturesEnabled
        {
            get { return _betaFeaturesEnabled; }
            set { _betaFeaturesEnabled = value; }
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


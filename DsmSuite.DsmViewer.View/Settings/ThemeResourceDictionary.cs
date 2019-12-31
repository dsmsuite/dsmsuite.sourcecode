using System;
using System.Windows;
using DsmSuite.DsmViewer.ViewModel.Settings;

namespace DsmSuite.DsmViewer.View.Settings
{
    public class ThemeResourceDictionary : ResourceDictionary
    {
        private Uri _lightThemeSource;
        private Uri _highContrastThemeSource;
        private Uri _darkThemeSource;

        public Uri LightThemeSource
        {
            get { return _lightThemeSource; }
            set
            {
                _lightThemeSource = value;
                UpdateSource();
            }
        }

        public Uri HighContrastThemeSource
        {
            get { return _highContrastThemeSource; }
            set
            {
                _highContrastThemeSource = value;
                UpdateSource();
            }
        }


        public Uri DarkThemeSource
        {
            get { return _darkThemeSource; }
            set
            {
                _darkThemeSource = value;
                UpdateSource();
            }
        }

        private void UpdateSource()
        {
            Uri uri;
            switch (App.Skin)
            {
                case Theme.Dark:
                    uri = DarkThemeSource;
                    break;
                case Theme.HighContrast:
                    uri = HighContrastThemeSource;
                    break;
                case Theme.Light:
                    uri = LightThemeSource;
                    break;
                default:
                    uri = LightThemeSource;
                    break;
            }

            if ((uri != null) && (Source != uri))
            {
                Source = uri;
            }
        }
    }
}

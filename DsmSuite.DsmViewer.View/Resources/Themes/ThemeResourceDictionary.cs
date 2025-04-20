using DsmSuite.DsmViewer.ViewModel.Settings;
using System.Windows;

namespace DsmSuite.DsmViewer.View.Resources.Themes
{
    public class ThemeResourceDictionary : ResourceDictionary
    {
        private Uri _lightThemeSource;
        private Uri _pastelThemeSource;
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

        public Uri PastelThemeSource
        {
            get { return _pastelThemeSource; }
            set
            {
                _pastelThemeSource = value;
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
                case Theme.Pastel:
                    uri = PastelThemeSource;
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

using System.Collections.Generic;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private const string DarkThemeName = "Dark";
        private const string HighContrastThemeName = "High contrast";
        private const string LightThemeName = "Light";

        private readonly IDsmApplication _application;
        private bool _showCycles;
        private bool _loggingEnabled;
        private readonly List<string> _supportedThemeNames;
        private string _selectedThemeName;

        public SettingsViewModel(IDsmApplication application)
        {
            _application = application;

            _supportedThemeNames = new List<string>()
            {
                LightThemeName,
                HighContrastThemeName,
                DarkThemeName
            };

            LoggingEnabled = ViewerSetting.LoggingEnabled;
            ShowCycles = ViewerSetting.ShowCycles;
            SelectedThemeName = ThemeToString(ViewerSetting.Theme);

            _application.ShowCycles = ShowCycles;

            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public ICommand AcceptChangeCommand { get; }

        public bool ShowCycles
        {
            get { return _showCycles; }
            set
            {
                _showCycles = value;
                OnPropertyChanged();
            }
        }

        public bool LoggingEnabled
        {
            get { return _loggingEnabled; }
            set
            {
                _loggingEnabled = value;
                OnPropertyChanged();
            }
        }

        public List<string> SupportedThemeNames => _supportedThemeNames;

        public string SelectedThemeName
        {
            get { return _selectedThemeName; }
            set
            {
                _selectedThemeName = value;
                OnPropertyChanged();
            }
        }

        private void AcceptChangeExecute(object parameter)
        {
            ViewerSetting.LoggingEnabled = LoggingEnabled;
            ViewerSetting.ShowCycles = ShowCycles;
            ViewerSetting.Theme = StringToTheme(SelectedThemeName);

            _application.ShowCycles = ShowCycles;
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }

        private static string ThemeToString(Theme theme)
        {
            switch (theme)
            {
                case Theme.Dark:
                    return DarkThemeName;
                case Theme.HighContrast:
                    return HighContrastThemeName;
                case Theme.Light:
                    return LightThemeName;
                default:
                    return LightThemeName;
            }
        }

        private static Theme StringToTheme(string text)
        {
            switch (text)
            {
                case DarkThemeName:
                    return Theme.Dark;
                case HighContrastThemeName:
                    return Theme.HighContrast;
                case LightThemeName:
                    return Theme.Light;
                default:
                    return Theme.Light;
            }
        }
    }
}

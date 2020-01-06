using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private const string DarkThemeName = "Dark";
        private const string PastelThemeName = "Pastel";
        private const string LightThemeName = "Light";

        private readonly IDsmApplication _application;
        private bool _showCycles;
        private bool _loggingEnabled;
        private bool _betaFeaturesEnabled;
        private string _selectedThemeName;

        private readonly Dictionary<Theme, string> _supportedThemes;

        public SettingsViewModel(IDsmApplication application)
        {
            _application = application;
            _supportedThemes = new Dictionary<Theme, string>();
            //_supportedThemes[Theme.Dark] = DarkThemeName;
            _supportedThemes[Theme.Pastel] = PastelThemeName;
            _supportedThemes[Theme.Light] = LightThemeName;

            LoggingEnabled = ViewerSetting.LoggingEnabled;
            ShowCycles = ViewerSetting.ShowCycles;
            BetaFeaturesEnabled = ViewerSetting.BetaFeaturesEnabled;
            SelectedThemeName = _supportedThemes[ViewerSetting.Theme];

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

        public bool BetaFeaturesEnabled
        {
            get { return _betaFeaturesEnabled; }
            set
            {
                _betaFeaturesEnabled = value;
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

        public List<string> SupportedThemeNames => _supportedThemes.Values.ToList();

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
            ViewerSetting.BetaFeaturesEnabled = BetaFeaturesEnabled;
            ViewerSetting.Theme = _supportedThemes.FirstOrDefault(x => x.Value == SelectedThemeName).Key;

            _application.ShowCycles = ShowCycles;
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}

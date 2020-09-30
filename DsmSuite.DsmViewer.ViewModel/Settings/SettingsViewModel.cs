using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private const string PastelThemeName = "Pastel";
        private const string LightThemeName = "Light";

        private readonly IDsmApplication _application;
        private bool _loggingEnabled;
        private string _selectedThemeName;

        private readonly Dictionary<Theme, string> _supportedThemes;

        public SettingsViewModel(IDsmApplication application)
        {
            _application = application;
            _supportedThemes = new Dictionary<Theme, string>
            {
                [Theme.Pastel] = PastelThemeName,
                [Theme.Light] = LightThemeName
            };

            LoggingEnabled = ViewerSetting.LoggingEnabled;
            SelectedThemeName = _supportedThemes[ViewerSetting.Theme];

            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public ICommand AcceptChangeCommand { get; }

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
            ViewerSetting.Theme = _supportedThemes.FirstOrDefault(x => x.Value == SelectedThemeName).Key;
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}

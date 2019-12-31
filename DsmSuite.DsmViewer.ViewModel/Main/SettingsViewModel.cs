using System.Collections.Generic;

using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class SettingsViewModel : ViewModelBase
    {
        private const string DarkTheme = "Dark";
        private const string HighContrastTheme = "High contrast";
        private const string LightTheme = "Light";

        private readonly IDsmApplication _application;
        private bool _showCycles;
        private bool _enableLogging;
        private readonly List<string> _supportedThemes;
        private string _selectedTheme;

        public SettingsViewModel(IDsmApplication application)
        {
            _application = application;
            ShowCycles = _application.ShowCycles;

            _supportedThemes = new List<string>()
            {
                LightTheme,
                HighContrastTheme,
                DarkTheme
            };
            _selectedTheme = _supportedThemes[0];
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

        public bool EnableLogging
        {
            get { return _enableLogging; }
            set
            {
                _enableLogging = value;
                OnPropertyChanged();
            }
        }

        public List<string> SupportedThemes => _supportedThemes;

        public string SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                _selectedTheme = value;
                OnPropertyChanged();
            }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.ShowCycles = ShowCycles;
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}

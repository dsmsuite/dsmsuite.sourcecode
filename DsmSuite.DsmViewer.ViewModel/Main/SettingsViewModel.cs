using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private bool _showCycles;

        public SettingsViewModel(IDsmApplication application)
        {
            _application = application;
            ShowCycles = _application.ShowCycles;
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public ICommand AcceptChangeCommand { get; }

        public bool ShowCycles
        {
            get { return _showCycles; }
            set { _showCycles = value; OnPropertyChanged(); }
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

using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing
{
    public class SnapshotMakeViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private string _description;

        public ICommand AcceptChangeCommand { get; }

        public SnapshotMakeViewModel(IDsmApplication application)
        {
            _application = application;

            Title = "Make snapshot";
            Description = "";
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged();  }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.MakeSnapshot(Description);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}

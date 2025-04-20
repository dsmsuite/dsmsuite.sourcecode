using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Snapshot
{
    public class SnapshotMakeViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private string _description;
        private string _help;

        public ICommand AcceptChangeCommand { get; }

        public SnapshotMakeViewModel(IDsmApplication application)
        {
            _application = application;

            Title = "Make snapshot";
            Help = "";

            Description = "";
            AcceptChangeCommand = RegisterCommand(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.MakeSnapshot(Description);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Description.Length > 0;
        }
    }
}

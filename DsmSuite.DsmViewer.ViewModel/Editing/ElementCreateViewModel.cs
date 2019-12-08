using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing
{
    public class ElementCreateViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _parent;
        private string _name;
        private string _type;

        public ICommand CreateElementCommand { get; }

        public ElementCreateViewModel(IDsmApplication application, IDsmElement parent)
        {
            _application = application;
            _parent = parent;

            Name = "";
            Type = "";

            CreateElementCommand = new RelayCommand<object>(CreateElementExecute, CreateElementCanExecute);
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        private void CreateElementExecute(object parameter)
        {
            _application.CreateElement(Name, Type, _parent);
        }

        private bool CreateElementCanExecute(object parameter)
        {
            return Name.Length > 0;
        }
    }
}

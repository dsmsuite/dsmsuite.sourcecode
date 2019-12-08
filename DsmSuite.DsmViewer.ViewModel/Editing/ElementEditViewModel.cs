using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing
{
    public class ElementEditViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _element;
        private string _name;
        private string _type;

        public ICommand EditElementCommand { get; }

        public ElementEditViewModel(IDsmApplication application, IDsmElement element)
        {
            _application = application;
            _element = element;
            Name = _element.Name;
            Type = _element.Type;
            EditElementCommand = new RelayCommand<object>(EditElementExecute, EditElementCanExecute);
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

        private void EditElementExecute(object parameter)
        {
            _application.EditElement(_element, Name, Type);
        }

        private bool EditElementCanExecute(object parameter)
        {
            return true;
        }
    }
}

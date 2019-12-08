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

        public ICommand AcceptChangeCommand { get; }

        public ElementEditViewModel(IDsmApplication application, IDsmElement element)
        {
            _application = application;
            _element = element;

            Title = $"Edit element {element.Fullname}";
            Name = _element.Name;
            Type = _element.Type;

            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

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

        private void AcceptChangeExecute(object parameter)
        {
            _application.EditElement(_element, Name, Type);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Name.Length > 0;
        }
    }
}

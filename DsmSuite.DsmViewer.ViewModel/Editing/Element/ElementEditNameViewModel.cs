using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Element
{
    public class ElementEditNameViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _element;
        private string _name;

        public ICommand AcceptChangeCommand { get; }

        public ElementEditNameViewModel(IDsmApplication application, IDsmElement element)
        {
            _application = application;
            _element = element;

            Title = $"Edit element name {element.Fullname}";
            Name = _element.Name;

            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.EditElementName(_element, Name);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Name.Length > 0;
        }
    }
}

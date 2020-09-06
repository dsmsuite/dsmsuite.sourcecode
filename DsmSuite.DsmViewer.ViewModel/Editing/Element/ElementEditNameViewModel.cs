using System.Windows.Input;
using DsmSuite.Common.Util;
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
        private string _help;

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

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.ChangeElementName(_element, Name);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            ElementName elementName = new ElementName(_element.Fullname);

            string newName = elementName.ParentName.Length > 0 ? elementName.ParentName + "." + Name : Name;
            string existingName = _element.Fullname;

            if (newName == existingName)
            {
                Help = "Name is unchanged";
                return false;
            }
            else if (Name.Length == 0)
            {
                Help = "Name is empty";
                return false;
            }
            else if (Name.Contains("."))
            {
                Help = "Name should not contain dot character";
                return false;
            }
            else if (_application.GetElementByFullname(newName) != null)
            {
                Help = "Name conflicts with existing name";
                return false;
            }
            else
            {
                Help = "";
                return true;
            }
        }
    }
}

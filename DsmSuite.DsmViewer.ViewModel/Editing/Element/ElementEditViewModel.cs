using System.Windows.Input;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Element
{
    public class ElementEditViewModel : ViewModelBase
    {
        private readonly ElementEditViewModelType _viewModelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _parentElement;
        private readonly IDsmElement _selectedElement;
        private string _name;
        private string _help;
        private string _type;

        public ICommand AcceptChangeCommand { get; }

        public ElementEditViewModel(ElementEditViewModelType viewModelType, IDsmApplication application, IDsmElement selectedElement)
        {
            _viewModelType = viewModelType;
            _application = application;

            switch (_viewModelType)
            {
                case ElementEditViewModelType.Modify:
                    Title = "Modify element";
                    _parentElement = selectedElement.Parent;
                    _selectedElement = selectedElement;
                    Name = _selectedElement.Name;
                    Type = _selectedElement.Type;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptModifyExecute, AcceptCanExecute);
                    break;
                case ElementEditViewModelType.Add:
                    Title = "Add element";
                    _parentElement = selectedElement;
                    _selectedElement = null;
                    Name = "";
                    Type = "";
                    AcceptChangeCommand = new RelayCommand<object>(AcceptAddExecute, AcceptCanExecute);
                    break;
                default:
                    break;
            }
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

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        private void AcceptAddExecute(object parameter)
        {
            _application.CreateElement(Name, Type, _parentElement);
        }

        private void AcceptModifyExecute(object parameter)
        {
            if (_selectedElement.Name != Name)
            {
                _application.ChangeElementName(_selectedElement, Name);
            }

            if (_selectedElement.Type != Type)
            {
                _application.ChangeElementType(_selectedElement, Type);
            }
        }

        private bool AcceptCanExecute(object parameter)
        {
            ElementName elementName = new ElementName(_parentElement.Fullname);
            elementName.AddNamePart(Name);

            if (Name.Length == 0)
            {
                Help = "Name can not be empty";
                return false;
            }
            else if (Name.Contains("."))
            {
                Help = "Name can not be contain dot character";
                return false;
            }
            else if (_application.GetElementByFullname(Name) != null)
            {
                Help = "Name can not be an existing name";
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

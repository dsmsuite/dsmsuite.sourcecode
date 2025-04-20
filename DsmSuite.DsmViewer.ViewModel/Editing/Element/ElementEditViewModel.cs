using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Element
{
    public class ElementEditViewModel : ViewModelBase
    {
        private readonly ElementEditViewModelType _viewModelType;
        private readonly IDsmApplication _application;
        private readonly IDsmElement _parentElement;
        private readonly IDsmElement _selectedElement;
        private readonly int _addAtIndex;
        private string _name;
        private string _help;
        private string _selectedElementType;

        private static string _lastSelectedElementType = "";

        public ICommand AcceptChangeCommand { get; }

        public ElementEditViewModel(ElementEditViewModelType viewModelType, IDsmApplication application, IDsmElement selectedElement)
        {
            _viewModelType = viewModelType;
            _application = application;

            ElementTypes = new List<string>(application.GetElementTypes());

            switch (_viewModelType)
            {
                case ElementEditViewModelType.Modify:
                    Title = "Modify element";
                    _parentElement = selectedElement.Parent;
                    _selectedElement = selectedElement;
                    _addAtIndex = 0;
                    Name = _selectedElement.Name;
                    SelectedElementType = _selectedElement.Type;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptModifyExecute, AcceptCanExecute);
                    break;
                case ElementEditViewModelType.AddChild:
                    Title = "Add element";
                    _parentElement = selectedElement;
                    _selectedElement = null;
                    _addAtIndex = _parentElement.Children.Count; // Insert at end
                    Name = "";
                    SelectedElementType = _lastSelectedElementType;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptAddExecute, AcceptCanExecute);
                    break;
                case ElementEditViewModelType.AddSiblingAbove:
                    Title = "Add element";
                    _parentElement = selectedElement.Parent;
                    _selectedElement = selectedElement;
                    _addAtIndex = _parentElement.IndexOfChild(_selectedElement);
                    Name = "";
                    SelectedElementType = _selectedElement.Type;
                    AcceptChangeCommand = new RelayCommand<object>(AcceptAddExecute, AcceptCanExecute);
                    break;
                case ElementEditViewModelType.AddSiblingBelow:
                    Title = "Add element";
                    _parentElement = selectedElement.Parent;
                    _selectedElement = selectedElement;
                    _addAtIndex = _parentElement.IndexOfChild(_selectedElement) + 1;
                    Name = "";
                    SelectedElementType = _selectedElement.Type;
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

        public List<string> ElementTypes { get; }

        public string SelectedElementType
        {
            get { return _selectedElementType; }
            set { _selectedElementType = value; _lastSelectedElementType = value; OnPropertyChanged(); }
        }

        private void AcceptAddExecute(object parameter)
        {
            _application.CreateElement(Name, SelectedElementType, _parentElement, _addAtIndex);
        }

        private void AcceptModifyExecute(object parameter)
        {
            if (_selectedElement.Name != Name)
            {
                _application.ChangeElementName(_selectedElement, Name);
            }

            if (_selectedElement.Type != SelectedElementType)
            {
                _application.ChangeElementType(_selectedElement, SelectedElementType);
            }
        }

        private bool AcceptCanExecute(object parameter)
        {
            ElementName elementName = new ElementName(_parentElement.Fullname);
            elementName.AddNamePart(Name);
            IDsmElement existingElement = _application.GetElementByFullname(elementName.FullName);

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
            else if ((existingElement != _selectedElement) && (existingElement != null))
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

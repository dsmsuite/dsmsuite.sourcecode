using System.Windows.Input;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Element
{
    public class ElementCreateViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _element;
        private string _name;
        private string _help;
        private string _type;

        public ICommand AcceptChangeCommand { get; }

        public ElementCreateViewModel(IDsmApplication application, IDsmElement element)
        {
            _application = application;
            _element = element;

            Title = "Create child element";
            Help = "";
            Name = "";
            Type = "";

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

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.CreateElement(Name, Type, _element);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            ElementName elementName = new ElementName(_element.Fullname);
            elementName.AddNamePart(Name);
            return (Name.Length > 0) && (_application.GetElementByFullname(elementName.FullName) == null);
        }
    }
}

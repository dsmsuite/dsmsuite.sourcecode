using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Element
{
    public class ElementEditAnnotationViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _element;
        private string _annotation;

        public ICommand AcceptChangeCommand { get; }

        public ElementEditAnnotationViewModel(IDsmApplication application, IDsmElement element)
        {
            _application = application;
            _element = element;

            Title = $"Edit element annotation";
            SubTitle = $"For element '{element.Fullname}'";

            IDsmElementAnnotation annotation = _application.FindElementAnnotation(element);
            Annotation = annotation?.Text;
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public string Annotation
        {
            get { return _annotation; }
            set { _annotation = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.ChangeElementAnnotation(_element, Annotation);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}


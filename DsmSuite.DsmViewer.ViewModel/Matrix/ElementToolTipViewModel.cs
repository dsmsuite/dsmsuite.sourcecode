using System.Runtime.InteropServices;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ElementToolTipViewModel : ViewModelBase
    {
        public ElementToolTipViewModel(IDsmElement element, IDsmApplication application)
        {
            Title = $"[{element.Id}] {element.Name}";
            Name = element.Fullname;
            Type = element.Type;

            IDsmElementAnnotation annotation = application?.FindElementAnnotation(element);
            if (annotation != null)
            {
                Annotation = annotation.Text;
            }
        }

        public string Title { get; }
        public string Name { get;  }
        public string Type { get; }
        public string Annotation { get; }
    }
}

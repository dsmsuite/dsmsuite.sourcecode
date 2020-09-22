using System.Runtime.InteropServices;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ElementToolTipViewModel : ViewModelBase
    {
        private int _id;
        private string _fullName;
        private string _type;
        private string _annotation;

        public ElementToolTipViewModel(int id, string fullName, string type, string annotation)
        {
            Id = id;
            FullName = fullName;
            Type = type;
            Annotation = annotation;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; OnPropertyChanged(); }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        public string Annotation
        {
            get { return _annotation; }
            set { _annotation = value; OnPropertyChanged(); }
        }
    }
}

using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ElementListItemViewModel : ViewModelBase
    {
        public ElementListItemViewModel(int index, IDsmElement element)
        {
            Index = index;
            ElementName = element.Fullname;
            ElementType = element.Type;
        }

        public int Index { get; }
        public string ElementName { get; }
        public string ElementType { get; }
    }
}

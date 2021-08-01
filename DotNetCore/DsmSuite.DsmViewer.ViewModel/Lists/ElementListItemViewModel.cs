using System;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ElementListItemViewModel : ViewModelBase, IComparable
    {
        public ElementListItemViewModel(IDsmElement element)
        {
            ElementName = element.Fullname;
            ElementType = element.Type;
        }

        public int Index { get; set; }
        public string ElementName { get; }
        public string ElementType { get; }

        public int CompareTo(object obj)
        {
            ElementListItemViewModel other = obj as ElementListItemViewModel;
            return string.Compare(ElementName, other?.ElementName, StringComparison.Ordinal);
        }
    }
}

using System;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ElementListItemViewModel : ViewModelBase, IComparable
    {
        private IDsmElement _element;

        public ElementListItemViewModel(IDsmElement element)
        {
            _element = element;
            ElementName = element.Name;
            ElementPath = element.Parent.Fullname;
            ElementType = element.Type;
            Properties = _element.Properties;
        }

        public int Index { get; set; }
        public string ElementPath { get; }
        public string ElementName { get; }
        public string ElementType { get; }
        public IDictionary<string, string> Properties { get; }

        public IEnumerable<string> DiscoveredElementPropertyNames()
        {
            return _element.DiscoveredElementPropertyNames();
        }

        public int CompareTo(object obj)
        {
            ElementListItemViewModel other = obj as ElementListItemViewModel;
            return string.Compare(ElementName, other?.ElementName, StringComparison.Ordinal);
        }
    }
}

using System;
using System.Collections.Generic;
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
            Properties = "";
            if (element.Properties != null)
            {
                foreach (KeyValuePair<string, string> elementProperty in element.Properties)
                {
                    Properties += string.Format("{0}={1} ", elementProperty.Key, elementProperty.Value);
                }
            }
        }

        public int Index { get; set; }
        public string ElementName { get; }
        public string ElementType { get; }
        public string Properties { get; }

        public int CompareTo(object obj)
        {
            ElementListItemViewModel other = obj as ElementListItemViewModel;
            return string.Compare(ElementName, other?.ElementName, StringComparison.Ordinal);
        }
    }
}

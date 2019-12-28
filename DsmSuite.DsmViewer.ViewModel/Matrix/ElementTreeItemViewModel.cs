using System.Collections.ObjectModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ElementTreeItemViewModel : ViewModelBase
    {
        public ElementTreeItemViewModel(MatrixViewModel matrixViewModel, IDsmElement element, int depth)
        {
            Children = new ObservableCollection<ElementTreeItemViewModel>();
            Element = element;
            Depth = depth;
            Color = MatrixColorConverter.GetColor(depth);

            MoveCommand = matrixViewModel.ChangeElementParentCommand;
            MoveUpElementCommand = matrixViewModel.MoveUpElementCommand;
            MoveDownElementCommand = matrixViewModel.MoveDownElementCommand;
            SortElementCommand = matrixViewModel.SortElementCommand;
            ToggleElementExpandedCommand = matrixViewModel.ToggleElementExpandedCommand;
        }
        
        public IDsmElement Element { get; }
        public int Depth { get; }
        public MatrixColor Color { get; }

        public int Id => Element.Id;
        public int Order => Element.Order;
        public string Name => Element.Name;
        public string Fullname => Element.Fullname;
        public string Description => $"[{Element.Order}] {Element.Fullname} size={Element.Size}";

        public ICommand MoveCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }
        public ICommand SortElementCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }

        public bool IsExpandable => Element.HasChildren;

        public bool IsExpanded
        {
            get
            {
                return Element.IsExpanded;
            }
            set
            {
                Element.IsExpanded = value;
            }
        }

        public ObservableCollection<ElementTreeItemViewModel> Children { get; }
        
        public int LeafElementCount
        {
            get
            {
                int count = 0;
                CountLeafElements(this, ref count);
                return count;
            }
        }

        private void CountLeafElements(ElementTreeItemViewModel element, ref int count)
        {
            if (element.Children.Count == 0)
            {
                count++;
            }
            else
            {
                foreach (ElementTreeItemViewModel child in element.Children)
                {
                    CountLeafElements(child, ref count);
                }
            }
        }
    }
}

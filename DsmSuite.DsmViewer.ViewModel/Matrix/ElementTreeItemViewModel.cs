using System.Collections.ObjectModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ElementTreeItemViewModel : ElementViewModel
    {
        private readonly MatrixViewModel _matrixViewModel;

        public ElementTreeItemViewModel(MatrixViewModel matrixViewModel, IDsmElement element, ElementRole role, int depth) :
            base(matrixViewModel, element, role, depth)
        {
            _matrixViewModel = matrixViewModel;
            Depth = depth;
            Children = new ObservableCollection<ElementTreeItemViewModel>();

            MoveCommand = _matrixViewModel.MoveCommand;
            MoveUpCommand = _matrixViewModel.MoveUpCommand;
            MoveDownCommand = _matrixViewModel.MoveDownCommand;
            PartitionCommand = _matrixViewModel.PartitionCommand;
            ToggleElementExpandedCommand = _matrixViewModel.ToggleElementExpandedCommand;
        }

        public int Depth { get; }

        public ICommand MoveCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand PartitionCommand { get; }
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

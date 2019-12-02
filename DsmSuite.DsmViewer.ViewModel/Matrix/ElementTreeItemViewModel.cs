using System.Collections.ObjectModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.Model;
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
            Children = new ObservableCollection<ElementTreeItemViewModel>();
            UpdateChildren();

            MoveUpCommand = _matrixViewModel.MoveUpCommand;
            MoveDownCommand = _matrixViewModel.MoveDownCommand;
            PartitionCommand = _matrixViewModel.PartitionCommand;
            ToggleElementExpandedCommand = _matrixViewModel.ToggleElementExpandedCommand;
        }

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
                UpdateChildren();
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
        private void UpdateChildren()
        {
            if (Element.IsExpanded)
            {
                foreach (IDsmElement child in Element.Children)
                {
                    Children.Add(new ElementTreeItemViewModel(_matrixViewModel, child, Role, Depth + 1));
                }
            }
            else
            {
                Children.Clear();
            }
        }


    }
}

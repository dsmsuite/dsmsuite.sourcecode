using System.Windows.Input;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Main;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ElementTreeItemViewModel : ViewModelBase
    {
        private readonly List<ElementTreeItemViewModel> _children;
        private ElementTreeItemViewModel _parent;

        public ElementTreeItemViewModel(IMainViewModel mainViewModel, IMatrixViewModel matrixViewModel, IDsmApplication application, IDsmElement element, int depth)
        {
            _children = new List<ElementTreeItemViewModel>();
            _parent = null;
            Element = element;
            Depth = depth;
            Color = MatrixColorConverter.GetColor(depth);

            MoveCommand = matrixViewModel.ChangeElementParentCommand;
            MoveUpElementCommand = matrixViewModel.MoveUpElementCommand;
            MoveDownElementCommand = matrixViewModel.MoveDownElementCommand;
            SortElementCommand = matrixViewModel.SortElementCommand;
            ToggleElementExpandedCommand = matrixViewModel.ToggleElementExpandedCommand;
            BookmarkElementCommand = matrixViewModel.ToggleElementBookmarkCommand;
            AnnotateElementCommand = matrixViewModel.ChangeElementAnnotationCommand;

            IndicatorViewMode = mainViewModel.SelectedIndicatorViewMode;

            ToolTipViewModel = new ElementToolTipViewModel(Element, application);
            HasAnnotation = ToolTipViewModel.HasAnnotation;
        }

        public ElementToolTipViewModel ToolTipViewModel { get; }
        public IDsmElement Element { get; }

        public int Depth { get; }
        public MatrixColor Color { get; }

        public int Id => Element.Id;
        public int Order => Element.Order;
        public bool IsConsumer { get; set; }
        public bool IsProvider { get; set; }
        public bool IsMatch => Element.IsMatch;
        public bool IsBookmarked => Element.IsBookmarked;
        public bool HasAnnotation { get; }

        public string Name => Element.IsRoot ? "Root" : Element.Name;

        public string Fullname => Element.Fullname;

        public ICommand MoveCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }
        public ICommand SortElementCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }
        public ICommand BookmarkElementCommand { get; }
        public ICommand AnnotateElementCommand { get; }

        public IndicatorViewMode IndicatorViewMode { get; }

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

        public IReadOnlyList<ElementTreeItemViewModel> Children => _children;

        public ElementTreeItemViewModel Parent => _parent;

        public void AddChild(ElementTreeItemViewModel viewModel)
        {
            _children.Add(viewModel);
            viewModel._parent = this;
        }

        public void ClearChildren()
        {
            foreach (ElementTreeItemViewModel viewModel in _children)
            {
                viewModel._parent = null;
            }
            _children.Clear();
        }

        public int LeafElementCount
        {
            get
            {
                int count = 0;
                CountLeafElements(this, ref count);
                return count;
            }
        }

        private void CountLeafElements(ElementTreeItemViewModel viewModel, ref int count)
        {
            if (viewModel.Children.Count == 0)
            {
                count++;
            }
            else
            {
                foreach (ElementTreeItemViewModel child in viewModel.Children)
                {
                    CountLeafElements(child, ref count);
                }
            }
        }
    }
}

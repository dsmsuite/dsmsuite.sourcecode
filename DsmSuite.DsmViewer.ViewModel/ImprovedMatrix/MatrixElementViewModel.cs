using System.Collections.ObjectModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.ImprovedMatrix
{
    public class MatrixElementViewModel :ViewModelBase
    {
        private readonly IDsmElement _element;
        private DsmColor _color;

        public MatrixElementViewModel(ImprovedMatrixViewModel matrixViewModel, IDsmElement element, int depth, DsmColor color)
        {
            _element = element;
            Depth = depth;
            Color = color;

            Children = new ObservableCollection<MatrixElementViewModel>();

            MoveCommand = matrixViewModel.ChangeElementParentCommand;
            MoveUpElementCommand = matrixViewModel.MoveUpElementCommand;
            MoveDownElementCommand = matrixViewModel.MoveDownElementCommand;
            SortElementCommand = matrixViewModel.SortElementCommand;
            ToggleElementExpandedCommand = matrixViewModel.ToggleElementExpandedCommand;
        }

        public IDsmElement Element => _element;
        public int Depth { get; }
        public DsmColor Color { get; }

        public ObservableCollection<MatrixElementViewModel> Children { get; }

        public ICommand MoveCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }
        public ICommand SortElementCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }

        public bool IsExpandable => _element.HasChildren;

        public bool IsExpanded
        {
            get
            {
                return _element.IsExpanded;
            }
            set
            {
                _element.IsExpanded = value;
            }
        }
    }
}
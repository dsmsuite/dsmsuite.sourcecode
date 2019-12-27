using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.ImprovedMatrix;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ImprovedElementTreeItemViewModel :ViewModelBase
    {
        private readonly IDsmElement _element;
        private readonly ElementRole _role;
        private int _color;

        public ImprovedElementTreeItemViewModel(ImprovedMatrixViewModel matrixViewModel, IDsmElement element, ElementRole role, int depth)
        {
            _element = element;
            _role = role;
            Depth = depth;
            Color = Math.Abs(Depth % 4);

            Children = new ObservableCollection<ImprovedElementTreeItemViewModel>();

            MoveCommand = matrixViewModel.ChangeElementParentCommand;
            MoveUpElementCommand = matrixViewModel.MoveUpElementCommand;
            MoveDownElementCommand = matrixViewModel.MoveDownElementCommand;
            SortElementCommand = matrixViewModel.SortElementCommand;
            ToggleElementExpandedCommand = matrixViewModel.ToggleElementExpandedCommand;
        }

        public ICommand MoveCommand { get; }
        public ICommand MoveUpElementCommand { get; }
        public ICommand MoveDownElementCommand { get; }
        public ICommand SortElementCommand { get; }
        public ICommand ToggleElementExpandedCommand { get; }

        public bool IsExpandable => _element.HasChildren;

        public int Depth { get; }

        public IDsmElement Element => _element;

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

        public int Color
        {
            get { return _color; }
            private set { _color = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ImprovedElementTreeItemViewModel> Children { get; }

        public int LeafElementCount
        {
            get
            {
                int count = 0;
                CountLeafElements(this, ref count);
                return count;
            }
        }

        private void CountLeafElements(ImprovedElementTreeItemViewModel element, ref int count)
        {
            if (element.Children.Count == 0)
            {
                count++;
            }
            else
            {
                foreach (ImprovedElementTreeItemViewModel child in element.Children)
                {
                    CountLeafElements(child, ref count);
                }
            }

        }
    }
}

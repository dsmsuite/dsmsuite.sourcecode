using System;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public enum ElementRole
    {
        Consumer,
        Provider
    }

    public class ElementViewModel : ViewModelBase
    {
        private readonly IDsmElement _element;
        private readonly ElementRole _role;
        private int _color;
        private bool _isSelected;
        private bool _isHovered;

        public ElementViewModel(MatrixViewModel matrixViewModel, IDsmElement element, ElementRole role, int depth)
        {
            _element = element;
            _role = role;
            Depth = depth;
            Color = Math.Abs(depth % 4);
            matrixViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
        }

        public int Color
        {
            get { return _color; }
            private set { _color = value; OnPropertyChanged(); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public bool IsHovered
        {
            get { return _isHovered; }
            set { _isHovered = value; OnPropertyChanged(); }
        }

        public int Id => _element.Id;
        public int Order => _element.Order;
        public string Name => _element.Name;
        public string Fullname => _element.Fullname;
        public int Depth { get; }
        public string Description => $"[{_element.Order}] {_element.Fullname}";
        public IDsmElement Element => _element;

        public ElementRole Role => _role;

        private void OnMainViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MatrixViewModel viewModel = sender as MatrixViewModel;
            if (viewModel != null)
            {
                switch (e.PropertyName)
                {
                    case nameof(MatrixViewModel.SelectedProvider):
                        if (_role == ElementRole.Provider)
                        {
                            IsSelected = (viewModel.SelectedProvider == this);
                        }
                        break;
                    case nameof(MatrixViewModel.HoveredProvider):
                        if (_role == ElementRole.Provider)
                        {
                            IsHovered = (viewModel.HoveredProvider == this);
                        }
                        break;
                    case nameof(MatrixViewModel.SelectedConsumer):
                        if (_role == ElementRole.Consumer)
                        {
                            IsSelected = (viewModel.SelectedConsumer == this);
                        }
                        break;
                    case nameof(MatrixViewModel.HoveredConsumer):
                        if (_role == ElementRole.Consumer)
                        {
                            IsHovered = (viewModel.HoveredConsumer == this);
                        }
                        break;
                }
            }
        }
    }
}

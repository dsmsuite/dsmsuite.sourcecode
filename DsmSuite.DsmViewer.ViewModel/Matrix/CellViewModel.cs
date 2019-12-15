using System;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class CellViewModel : ViewModelBase
    {
        private int _color;
        private bool _isSelected;
        private bool _isHovered;

        public CellViewModel(MatrixViewModel matrixViewModel, ElementViewModel consumer, ElementViewModel provider, int weight, bool cyclic, int row, int column, int color)
        {
            Consumer = consumer;
            Provider = provider;
            Weight = weight;
            Cyclic = cyclic;
            Row = row;
            Column = column;
            Color = color;

            matrixViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
        }

        private void OnMainViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MatrixViewModel viewModel = sender as MatrixViewModel;
            if (viewModel != null)
            {
                switch (e.PropertyName)
                {
                    case nameof(MatrixViewModel.SelectedProvider):
                    case nameof(MatrixViewModel.SelectedConsumer):
                        IsSelected = (viewModel.SelectedProvider == Provider) || (viewModel.SelectedConsumer == Consumer);
                        break;
                    case nameof(MatrixViewModel.HoveredProvider):
                    case nameof(MatrixViewModel.HoveredConsumer):
                        IsHovered = (viewModel.HoveredProvider == Provider) || (viewModel.HoveredConsumer == Consumer);
                        break;
                }
            }
        }

        public int ConsumerId => Consumer.Id;
        public int ProviderId => Provider.Id;

        public string Description => GetDescription();

        public ElementViewModel Consumer { get; }
        public ElementViewModel Provider { get; }
        public int Weight { get; }
        public bool Cyclic { get; }
        public int Row { get; }
        public int Column { get; }
        public bool IdentityCell => Consumer.Id == Provider.Id;

        public int Color
        {
            get { return _color; }
            private set { _color = value; OnPropertyChanged(); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            private set { _isSelected = value; OnPropertyChanged(); }
        }

        public bool IsHovered
        {
            get { return _isHovered; }
            private set { _isHovered = value; OnPropertyChanged(); }
        }

        private string GetDescription()
        {
            return $"Consumer={Consumer.Fullname} Provider={Provider.Fullname} Weight={Weight}";
        }
    }
}

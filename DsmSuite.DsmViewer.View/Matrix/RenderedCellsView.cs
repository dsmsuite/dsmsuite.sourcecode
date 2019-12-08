using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class RenderedCellsView : Canvas
    {
        private MatrixViewModel _matrixViewModel;
        private readonly RenderTheme _renderTheme;

        public RenderedCellsView()
        {
            _renderTheme = new RenderTheme(this);
            DataContextChanged += OnDataContextChanged;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseLeave += OnMouseLeave;
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            _matrixViewModel = DataContext as MatrixViewModel;
            if (_matrixViewModel != null)
            {
                _matrixViewModel.PropertyChanged += OnPropertyChanged;
                CreateChildViews();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            CellViewModel cellViewModel = GetCellViewModel(e.Source);
            if (cellViewModel != null)
            {
                _matrixViewModel?.HoverCell(cellViewModel.Consumer, cellViewModel.Provider);
            }
        }
        
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _matrixViewModel?.HoverCell(null, null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            CellViewModel cellViewModel = GetCellViewModel(e.Source);
            if (cellViewModel != null)
            {
                _matrixViewModel?.SelectCell(cellViewModel.Consumer, cellViewModel.Provider);
            }
        }
        
        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewModel.Dependencies))
            {
                CreateChildViews();
            }
        }

        private void CreateChildViews()
        {
            double size = 0.0;

            Children.Clear();
            if (_matrixViewModel?.Dependencies != null)
            {
                foreach (IList<CellViewModel> rowViewModel in _matrixViewModel.Dependencies)
                {
                    size += _renderTheme.MatrixCellSize + 2.0;

                    foreach (CellViewModel cellViewModel in rowViewModel)
                    {
                        RenderedCellView cellView = new RenderedCellView(_renderTheme) {DataContext = cellViewModel};
                        Children.Add(cellView);
                    }
                }
            }

            Height = size;
            Width = size;
        }

        private CellViewModel GetCellViewModel(object source)
        {
            RenderedCellView cellView = source as RenderedCellView;
            return cellView?.DataContext as CellViewModel;
        }
    }
}

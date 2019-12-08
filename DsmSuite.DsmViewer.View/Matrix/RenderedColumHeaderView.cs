using System.Windows.Controls;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class RenderedColumHeaderView : Canvas
    {
        private MatrixViewModel _matrixViewModel;
        private readonly RenderTheme _renderTheme;

        public RenderedColumHeaderView()
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
            ElementViewModel elementViewModel = GetElementViewModel(e.Source);
            if (elementViewModel != null)
            {
                _matrixViewModel?.HoverConsumer(elementViewModel);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _matrixViewModel?.HoverCell(null, null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ElementViewModel elementViewModel = GetElementViewModel(e.Source);
            if (elementViewModel != null)
            {
                _matrixViewModel?.SelectConsumer(elementViewModel);
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
            double width = 0.0;

            Children.Clear();
            int column = 0;
            if (_matrixViewModel?.Consumers != null)
            {
                foreach (ElementViewModel elementViewModel in _matrixViewModel.Consumers)
                {
                    RenderedColumHeaderItemView elementView = new RenderedColumHeaderItemView(_renderTheme, column)
                    {
                        DataContext = elementViewModel
                    };
                    Children.Add(elementView);

                    column++;

                    width += _renderTheme.MatrixCellSize + 2.0;
                }
            }

            Height = _renderTheme.MatrixHeaderHeight;
            Width = width;
        }

        private ElementViewModel GetElementViewModel(object source)
        {
            RenderedColumHeaderItemView headerItemView = source as RenderedColumHeaderItemView;
            return headerItemView?.DataContext as ElementViewModel;
        }
    }
}

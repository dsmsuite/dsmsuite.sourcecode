using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class RenderedRowHeaderView : Canvas
    {
        private MatrixViewModel _matrixViewModel;
        private readonly RenderTheme _renderTheme;

        public RenderedRowHeaderView()
        {
            _renderTheme = new RenderTheme(this);

            DataContextChanged += OnDataContextChanged;
            SizeChanged += OnSizeChanged;
            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseLeave += OnMouseLeave;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _matrixViewModel = DataContext as MatrixViewModel;
            if (_matrixViewModel != null)
            {
                _matrixViewModel.PropertyChanged += OnPropertyChanged;
                CreateChildViews();
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CreateChildViews();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            ElementTreeItemViewModel elementViewModel = GetElementViewModel(e.Source);
            if (elementViewModel != null)
            {
                _matrixViewModel?.HoverProviderTreeItem(elementViewModel);
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _matrixViewModel?.HoverProviderTreeItem(null);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            RenderedRowHeaderItemView headerItemView = e.Source as RenderedRowHeaderItemView;
            if (headerItemView != null)
            {
                Point pt = e.GetPosition(headerItemView);

                if ((pt.X < 20) && (pt.Y < 24))
                {
                    _matrixViewModel.ToggleElementExpandedCommand.Execute(null);
                    InvalidateVisual();
                }
            }

            ElementTreeItemViewModel elementViewModel = GetElementViewModel(e.Source);
            if (elementViewModel != null)
            {
                _matrixViewModel?.SelectProviderTreeItem(elementViewModel);
            }
        }

        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MatrixViewModel.Providers))
            {
                CreateChildViews();
            }

            if ((e.PropertyName == nameof(MatrixViewModel.SelectedRow)) ||
                (e.PropertyName == nameof(MatrixViewModel.SelectedRow)))
            {
                RedrawChildViews();
            }
        }

        private void RedrawChildViews()
        {
            foreach (var child in Children)
            {
                RenderedRowHeaderItemView renderedRowHeaderItemView = child as RenderedRowHeaderItemView;
                renderedRowHeaderItemView?.Redraw();
            }
        }
        
        private void CreateChildViews()
        {
            double y = 0.0;

            Children.Clear();
            if (_matrixViewModel?.Providers != null)
            {
                foreach (ElementTreeItemViewModel elementViewModel in _matrixViewModel.Providers)
                {
                    Rect rect = GetCalculatedSize(elementViewModel, y);

                    RenderedRowHeaderItemView elementView = new RenderedRowHeaderItemView(_matrixViewModel, _renderTheme)
                    {
                        Height = rect.Height,
                        Width = rect.Width
                    };
                    SetTop(elementView, rect.Top);
                    SetLeft(elementView, rect.Left);
                    elementView.DataContext = elementViewModel;
                    Children.Add(elementView);

                    CreateChildViews(elementViewModel, y);

                    y += rect.Height;
                }
            }

            Height = y;
            Width = 5000; // Should be enough to draw very deep tree
        }

        private void CreateChildViews(ElementTreeItemViewModel elementViewModel, double y)
        {
            foreach (ElementTreeItemViewModel child in elementViewModel.Children)
            {
                Rect rect = GetCalculatedSize(child, y);

                RenderedRowHeaderItemView elementView = new RenderedRowHeaderItemView(_matrixViewModel, _renderTheme)
                {
                    Height = rect.Height,
                    Width = rect.Width
                };
                SetTop(elementView, rect.Top);
                SetLeft(elementView, rect.Left);
                elementView.DataContext = child;
                Children.Add(elementView);

                CreateChildViews(child, y);

                y += rect.Height;
            }
        }

        private ElementTreeItemViewModel GetElementViewModel(object source)
        {
            RenderedRowHeaderItemView headerItemView = source as RenderedRowHeaderItemView;
            return headerItemView?.DataContext as ElementTreeItemViewModel;
        }

        private Rect GetCalculatedSize(ElementTreeItemViewModel viewModel, double y)
        {
            Rect rect = new Rect();
            if (viewModel != null)
            {
                int leafElementCount = viewModel.LeafElementCount;

                double pitch = _renderTheme.MatrixCellSize + 2.0;
                if (viewModel.IsExpanded)
                {
                    double x = viewModel.Depth * 26.0;
                    double width = pitch;
                    if (width > ActualWidth)
                    {
                        width = ActualWidth;
                    }
                    double height = leafElementCount * pitch;
                    rect = new Rect(x, y, width, height);
                }
                else
                {
                    double x = viewModel.Depth * 26.0;
                    double width = ActualWidth - x;
                    if (width < 0)
                    {
                        width = 0;
                    }
                    double height = pitch;
                    rect = new Rect(x, y, width, height);
                }
            }
            return rect;
        }
    }
}


using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Matrix;

namespace DsmSuite.DsmViewer.View.Matrix
{
    public class MatrixRowHeaderItemView : MatrixFrameworkElement
    {
        private readonly MatrixViewModel _matrixViewModel;
        private static readonly string DataObjectName = "Element";
        private static readonly string BlackRightPointingTriangle = '\u25B6'.ToString();
        private static readonly string BlackDownPointingTriangle = '\u25BC'.ToString();
        private static readonly FormattedText BlackRightPointingTriangleFormattedText;
        private static readonly FormattedText BlackDownPointingTriangleFormattedText;

        private readonly MatrixTheme _theme;
        private ElementTreeItemViewModel _viewModel;

        static MatrixRowHeaderItemView()
        {
            BlackRightPointingTriangleFormattedText = new FormattedText(BlackRightPointingTriangle,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                10,
                Brushes.Black);

            BlackDownPointingTriangleFormattedText = new FormattedText(BlackDownPointingTriangle,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                10,
                Brushes.Black);
        }

        public MatrixRowHeaderItemView(MatrixViewModel matrixViewModel, MatrixTheme theme)
        {
            _matrixViewModel = matrixViewModel;
            _matrixViewModel.PropertyChanged += OnMatrixViewModelPropertyChanged;
            _theme = theme;

            AllowDrop = true;
            
            DataContextChanged += OnDataContextChanged;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData(DataObjectName, _viewModel.Element);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.None);
            }
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            if (e.Data.GetDataPresent(DataObjectName))
            {
                IDsmElement element = (IDsmElement) e.Data.GetData(DataObjectName);
                IDsmElement newParent = _viewModel.Element;

                if ((element != null) && 
                    (newParent != null) && 
                    (element != newParent)) // Not dragged on itself
                {
                    Tuple<IDsmElement, IDsmElement> moveParameter = new Tuple<IDsmElement, IDsmElement>(element,newParent);
                    _viewModel.MoveCommand.Execute(moveParameter);
                }

                e.Effects = DragDropEffects.Move;
            }
            e.Handled = true;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewModel = e.NewValue as ElementTreeItemViewModel;
            if (_viewModel != null)
            {
                ToolTip = _viewModel.Description;
            }
        }

        public void Redraw()
        {
            InvalidateVisual();
        }

        private void OnMatrixViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == nameof(MatrixViewModel.SelectedRow)) ||
                (e.PropertyName == nameof(MatrixViewModel.HoveredRow)))
            {
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            if ((_viewModel != null) && (ActualWidth > 2.0) && (ActualHeight > 2.0))
            {
                bool isHovered = _matrixViewModel.HoveredProviderTreeItem == _viewModel;
                bool isSelected = _matrixViewModel.SelectedProviderTreeItem == _viewModel;
                SolidColorBrush background = _theme.GetBackground(_viewModel.Color, isHovered, isSelected);
                Rect backgroundRect = new Rect(1.0, 1.0, ActualWidth - 2.0, ActualHeight - 2.0);
                dc.DrawRectangle(background, null, backgroundRect);

                string text = _viewModel.Name + " - " + _viewModel.Order;
                if (_viewModel.IsExpanded)
                {
                    Point textLocation = new Point(backgroundRect.X + 10.0, backgroundRect.Y - 20.0);
                    DrawRotatedText(dc, textLocation, text, backgroundRect.Height - 20.0);
                }
                else
                {
                    Point textLocation = new Point(backgroundRect.X + 20.0, backgroundRect.Y + 15.0);
                    DrawText(dc, textLocation, text, backgroundRect.Width - 25.0);
                }

                Point expanderLocation = new Point(backgroundRect.X + 1.0, backgroundRect.Y + 1.0);
                DrawExpander(dc, expanderLocation);
            }
        }

        private void DrawExpander(DrawingContext dc, Point location)
        {
            if (_viewModel.IsExpandable)
            {
                dc.DrawText(
                    _viewModel.IsExpanded
                        ? BlackDownPointingTriangleFormattedText
                        : BlackRightPointingTriangleFormattedText, location);
            }
        }
    }
}
using DsmSuite.DsmViewer.ViewModel.Lists;
using System.Windows;

namespace DsmSuite.DsmViewer.View.Lists
{
    /// <summary>
    /// Interaction logic for ElementListView.xaml
    /// </summary>
    public partial class ElementListView
    {
        private ElementListViewModel _viewModel;

        public ElementListView()
        {
            InitializeComponent();
        }

        private void ElementListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as ElementListViewModel;
        }
    }
}

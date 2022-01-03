
using DsmSuite.DsmViewer.ViewModel.Lists;
using System.Windows;

namespace DsmSuite.DsmViewer.View.Lists
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class ActionListView
    {
        private ActionListViewModel _viewModel;

        public ActionListView()
        {
            InitializeComponent();
        }

        private void ActionListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as ActionListViewModel;
        }
    }
}

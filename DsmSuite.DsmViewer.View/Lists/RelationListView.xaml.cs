using DsmSuite.DsmViewer.ViewModel.Editing.Relation;
using DsmSuite.DsmViewer.View.Editing;
using DsmSuite.DsmViewer.ViewModel.Lists;
using System.Windows;

namespace DsmSuite.DsmViewer.View.Lists
{
    /// <summary>
    /// Interaction logic for RelationListView.xaml
    /// </summary>
    public partial class RelationListView
    {
        private RelationListViewModel _viewModel;

        public RelationListView()
        {
            InitializeComponent();
        }

        private void RelationListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as RelationListViewModel;
            _viewModel.RelationAddStarted += OnRelationAddStarted;
            _viewModel.RelationEditStarted += OnRelationEditStarted;
        }

        private void OnRelationAddStarted(object sender, RelationEditViewModel viewModel)
        {
            RelationEditDialog view = new RelationEditDialog { DataContext = viewModel };
            view.ShowDialog();
        }

        private void OnRelationEditStarted(object sender, RelationEditViewModel viewModel)
        {
            RelationEditDialog view = new RelationEditDialog { DataContext = viewModel };
            view.ShowDialog();
        }
    }
}

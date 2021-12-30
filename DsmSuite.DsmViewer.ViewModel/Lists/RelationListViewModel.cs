using System.Collections.Generic;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Interfaces;
using System.Collections.ObjectModel;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListViewModel : ViewModelBase
    {
        private IDsmApplication _application;

        public RelationListViewModel(string subtitle, IEnumerable<IDsmRelation> relations, IDsmApplication application)
        {
            _application = application;

            Title = "Relation List";
            SubTitle = subtitle;

            List<RelationListItemViewModel> relationViewModels = new List<RelationListItemViewModel>();

            foreach (IDsmRelation relation in relations)
            {
                relationViewModels.Add(new RelationListItemViewModel(relation));
            }

            relationViewModels.Sort();

            int index = 1;
            foreach (RelationListItemViewModel viewModel in relationViewModels)
            {
                viewModel.Index = index;
                index++;
            }

            Relations = new ObservableCollection<RelationListItemViewModel>(relationViewModels);

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
            DeleteCommand = new RelayCommand<object>(DeleteExecute, DeleteCanExecute);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public ObservableCollection<RelationListItemViewModel> Relations { get; }

        public RelationListItemViewModel SelectedRelation { get; set; }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand DeleteCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RelationListItemViewModel viewModel in Relations)
            {
                builder.AppendLine($"{viewModel.Index,-5}, {viewModel.ConsumerName,-100}, {viewModel.ProviderName,-100}, {viewModel.RelationType,-30}, {viewModel.RelationWeight,-10}, {viewModel.Properties,-150}");
            }
            Clipboard.SetText(builder.ToString());
        }

        private void DeleteExecute(object parameter)
        {
            _application.DeleteRelation(SelectedRelation.Relation);
            Relations.Remove(SelectedRelation);
        }

        private bool DeleteCanExecute(object parameter)
        {
            return SelectedRelation != null;
        }
    }
}

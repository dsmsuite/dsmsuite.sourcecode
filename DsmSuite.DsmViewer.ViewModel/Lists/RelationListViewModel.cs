using System.Collections.Generic;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Interfaces;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListViewModel : ViewModelBase
    {
        public RelationListViewModel(string subtitle, IEnumerable<IDsmRelation> relations, IDsmApplication application)
        {
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

            Relations = relationViewModels;

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public List<RelationListItemViewModel> Relations { get; }

        public ICommand CopyToClipboardCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RelationListItemViewModel viewModel in Relations)
            {
                builder.AppendLine($"{viewModel.Index, -5}, {viewModel.ConsumerName, -100}, {viewModel.ProviderName, -100}, {viewModel.RelationType, -30}, {viewModel.RelationWeight, -10}, {viewModel.Properties,-150}");
            }
            Clipboard.SetText(builder.ToString());
        }
    }
}

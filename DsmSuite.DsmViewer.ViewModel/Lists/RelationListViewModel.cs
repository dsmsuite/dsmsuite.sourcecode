using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;
using System.Text;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListViewModel : ViewModelBase
    {
        public RelationListViewModel(string title, IEnumerable<IDsmRelation> relations)
        {
            Title = title;

            var relationViewModels = new List<RelationListItemViewModel>();

            int index = 1;
            foreach (IDsmRelation relation in relations)
            {
                relationViewModels.Add(new RelationListItemViewModel(index, relation));
                index++;
            }

            Relations = relationViewModels;

            CopyToClipboardCommand = new RelayCommand<object>(CopyToClipboardExecute);
        }

        public string Title { get; }

        public List<RelationListItemViewModel> Relations { get; }

        public ICommand CopyToClipboardCommand { get; }

        private void CopyToClipboardExecute(object parameter)
        {
            StringBuilder builder = new StringBuilder();
            foreach (RelationListItemViewModel viewModel in Relations)
            {
                builder.AppendLine($"{viewModel.Index, -5}, {viewModel.ConsumerName, -100} {viewModel.ProviderName, -100} {viewModel.RelationType, -30} {viewModel.RelationWeight, -10}");
            }
            Clipboard.SetText(builder.ToString());
        }
    }
}

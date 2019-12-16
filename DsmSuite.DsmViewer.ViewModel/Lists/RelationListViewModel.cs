using System.Collections.Generic;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;
using System.Windows.Input;
using System.Windows;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class RelationListViewModel : ViewModelBase
    {
        public RelationListViewModel(string title, IEnumerable<IDsmResolvedRelation> relations)
        {
            Title = title;

            var relationViewModels = new List<RelationListItemViewModel>();

            int index = 1;
            foreach (IDsmResolvedRelation relation in relations)
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
            Clipboard.SetText("Copy relations");
        }
    }
}

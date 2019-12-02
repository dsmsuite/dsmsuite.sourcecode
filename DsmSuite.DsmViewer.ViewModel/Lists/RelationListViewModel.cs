using System.Collections.Generic;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

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
        }

        public string Title { get; }

        public List<RelationListItemViewModel> Relations { get; }
    }
}

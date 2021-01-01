using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Relation
{
    public class RelationEditWeightViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmRelation _relation;
        private int _weight;

        public ICommand AcceptChangeCommand { get; }

        public RelationEditWeightViewModel(IDsmApplication application, IDsmRelation relation)
        {
            _application = application;
            _relation = relation;

            Title = "Edit relation";
            SubTitle = "Edit relation";

            Weight = relation.Weight;
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }
        public string SubTitle { get; }

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.ChangeRelationWeight(_relation, Weight);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Weight > 0;
        }
    }
}

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
        private string _help;

        public ICommand AcceptChangeCommand { get; }

        public RelationEditWeightViewModel(IDsmApplication application, IDsmRelation relation)
        {
            _application = application;
            _relation = relation;

            Title = "Change relation weight";
            Help = "";

            Weight = relation.Weight;
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

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
            if (Weight == _relation.Weight)
            {
                return false;
            }
            else if (Weight < 0)
            {
                Help = "Weight can not be negative";
                return false;
            }
            else if (Weight == 0)
            {
                Help = "Weight can not be zero";
                return false;
            }
            else
            {
                Help = "";
                return true;
            }
        }
    }
}

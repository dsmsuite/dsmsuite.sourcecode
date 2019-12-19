using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing
{
    public class RelationEditViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmRelation _relation;
        private string _type;
        private int _weight;

        public ICommand AcceptChangeCommand { get; }

        public RelationEditViewModel(IDsmApplication application, IDsmRelation relation)
        {
            _application = application;
            _relation = relation;

            Title = "Edit relation";
            Type = relation.Type;
            Weight = relation.Weight;
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        public int Weight
        {
            get { return _weight; }
            set { _weight = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.EditRelation(_relation, Type, Weight);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Weight > 0;
        }
    }
}

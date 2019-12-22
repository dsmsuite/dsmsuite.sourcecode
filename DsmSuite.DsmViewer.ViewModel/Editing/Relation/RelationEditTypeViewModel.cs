using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Relation
{
    public class RelationEditTypeViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmRelation _relation;
        private string _type;

        public ICommand AcceptChangeCommand { get; }

        public RelationEditTypeViewModel(IDsmApplication application, IDsmRelation relation)
        {
            _application = application;
            _relation = relation;

            Title = "Edit relation type";
            Type = relation.Type;
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Type
        {
            get { return _type; }
            set { _type = value; OnPropertyChanged(); }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.ChangeRelationType(_relation, Type);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return true;
        }
    }
}

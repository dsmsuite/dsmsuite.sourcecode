using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;


namespace DsmSuite.DsmViewer.ViewModel.Editing.Relation
{
    public class RelationEditViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmRelation _relation; 
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private string _type;
        private int _weight;
        private string _help;

        public RelationEditViewModel(IDsmApplication application, IDsmRelation relation, IDsmElement consumer, IDsmElement provider)
        {
            _application = application;
            _relation = relation;
            _consumer = consumer;
            _provider = provider;
        }

        public string Title { get; }

        public string Help
        {
            get { return _help; }
            private set { _help = value; OnPropertyChanged(); }
        }

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
    }
}

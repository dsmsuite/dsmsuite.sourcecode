using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing
{
    public class RelationCreateViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private readonly IDsmElement _consumer;
        private readonly IDsmElement _provider;
        private string _type;
        private int _weight;

        public ICommand AcceptChangeCommand { get; }

        public RelationCreateViewModel(IDsmApplication application, IDsmElement consumer, IDsmElement provider)
        {
            _application = application;
            _consumer = consumer;
            _provider = provider;

            Title = $"Create relation between {consumer.Fullname} and {provider.Fullname}";
            Type = "";
            Weight = 1;
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
            _application.CreateRelation(_consumer, _provider, Type, Weight);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Weight > 0;
        }
    }
}

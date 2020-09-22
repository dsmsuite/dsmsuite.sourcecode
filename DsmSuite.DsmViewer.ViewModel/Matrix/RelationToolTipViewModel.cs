using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class RelationToolTipViewModel : ViewModelBase
    {
        private int _consumerId;
        private string _consumerName;
        private int _providerId;
        private string _providerName;

        private string _type;
        private string _weight;

        public int ConsumerId
        {
            get { return _consumerId; }
            set { _consumerId = value; OnPropertyChanged(); }
        }

        public string ConsumerName
        {
            get { return _consumerName; }
            set { _consumerName = value; OnPropertyChanged(); }
        }

        public int ProviderId
        {
            get { return _providerId; }
            set { _providerId = value; OnPropertyChanged(); }
        }

        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; OnPropertyChanged(); }
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

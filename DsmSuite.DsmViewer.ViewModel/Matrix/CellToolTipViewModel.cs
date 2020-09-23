using System;
using System.Windows.Forms;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class CellToolTipViewModel : ViewModelBase
    {
        public CellToolTipViewModel(IDsmElement consumer, IDsmElement provider, int weight, CycleType cycleType, IDsmApplication application)
        {
            Title = $"Relation {consumer.Name} - {provider.Name}";
            ConsumerId = consumer.Id;
            ConsumerName = consumer.Fullname;
            ProviderId = provider.Id;
            ProviderName = provider.Fullname; 
            Weight = weight;
            CycleType = cycleType.ToString();

            IDsmRelationAnnotation annotation = application?.FindRelationAnnotation(consumer, provider);
            if (annotation != null)
            {
                Annotation = annotation.Text;
            }
        }

        public string Title { get; }
        public int ConsumerId { get; }
        public string ConsumerName { get; }
        public int ProviderId { get; }
        public string ProviderName { get; }
        public int Weight { get; }
        public string CycleType { get; }
        public string Annotation { get; }
    }
}

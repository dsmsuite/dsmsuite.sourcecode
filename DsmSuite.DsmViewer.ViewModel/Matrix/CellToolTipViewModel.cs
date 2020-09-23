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
            Title = $"[{consumer.Id}] {consumer.Name} - [{provider.Id}] {provider.Name}";
            Consumer = consumer.Fullname;
            Provider = provider.Fullname;
            Weight = weight;
            CycleType = cycleType.ToString();

            IDsmRelationAnnotation annotation = application?.FindRelationAnnotation(consumer, provider);
            if (annotation != null)
            {
                Annotation = annotation.Text;
            }
        }

        public string Title { get; }
        public string Consumer { get; }
        public string Provider { get; }
        public int Weight { get; }
        public string CycleType { get; }
        public string Annotation { get; }
    }
}

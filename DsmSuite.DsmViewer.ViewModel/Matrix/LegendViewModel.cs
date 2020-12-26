using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class LegendViewModel : ViewModelBase
    {
        public LegendViewModel(LegendColor color, string description)
        {
            Color = color;
            Description = description;
        }

        public LegendColor Color { get; }
        public string Description { get; }
    }
}

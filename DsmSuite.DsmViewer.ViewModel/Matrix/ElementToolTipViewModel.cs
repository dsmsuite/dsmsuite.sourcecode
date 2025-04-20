using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public class ElementToolTipViewModel : ViewModelBase
    {
        public ElementToolTipViewModel(IDsmElement element, IDsmApplication application)
        {
            Title = $"Element {element.Name}";
            Id = element.Id;
            Name = element.Fullname;
            Type = element.Type;

            Legend = new List<LegendViewModel>();
            Legend.Add(new LegendViewModel(LegendColor.Consumer, "Consumer"));
            Legend.Add(new LegendViewModel(LegendColor.Provider, "Provider"));
            Legend.Add(new LegendViewModel(LegendColor.Cycle, "Cycle"));
            Legend.Add(new LegendViewModel(LegendColor.Search, "Search"));
            Legend.Add(new LegendViewModel(LegendColor.Bookmark, "Bookmark"));
        }

        public string Title { get; }
        public int Id { get; }
        public string Name { get; }
        public string Type { get; }
        public List<LegendViewModel> Legend { get; }
    }
}

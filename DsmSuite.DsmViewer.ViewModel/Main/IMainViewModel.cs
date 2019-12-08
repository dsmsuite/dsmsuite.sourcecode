using System;
using System.ComponentModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Lists;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public class ReportViewModel : ViewModelBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public enum SearchState
    {
        NoMatch,
        OneMatch,
        ManyMatches
    }

    public interface IMainViewModel : INotifyPropertyChanged
    {
        void NotifyReportCreated(ReportViewModel report);
        void NotifyElementsReportReady(ElementListViewModel report);
        void NotifyRelationsReportReady(RelationListViewModel report);

        event EventHandler<ReportViewModel> ReportCreated;
        event EventHandler<ElementListViewModel> ElementsReportReady;
        event EventHandler<RelationListViewModel> RelationsReportReady;

        ICommand ToggleElementExpandedCommand { get; }
        ICommand MoveUpElementCommand { get; }
        ICommand MoveDownElementCommand { get; }
        ICommand PartitionElementCommand { get; }
        ICommand ShowElementDetailMatrixCommand { get; }
        ICommand ShowElementContextMatrixCommand { get; }
        ICommand ShowCellDetailMatrixCommand { get; }
    }
}

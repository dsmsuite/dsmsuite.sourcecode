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

    public interface IMainViewModel : INotifyPropertyChanged
    {
        void NotifyReportCreated(ReportViewModel report);
        void NotifyElementsReportReady(ElementListViewModel report);
        void NotifyRelationsReportReady(RelationListViewModel report);

        event EventHandler<ReportViewModel> ReportCreated;
        event EventHandler<ElementListViewModel> ElementsReportReady;
        event EventHandler<RelationListViewModel> RelationsReportReady;

        ICommand ToggleElementExpandedCommand { get; }
        ICommand MoveUpCommand { get; }
        ICommand MoveDownCommand { get; }
        ICommand PartitionCommand { get; }
        ICommand ElementDetailMatrixCommand { get; }

        ICommand RelationDetailMatrixCommand { get; }
    }
}

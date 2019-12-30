using System;
using System.ComponentModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.ViewModel.Common;
using DsmSuite.DsmViewer.ViewModel.Editing.Element;
using DsmSuite.DsmViewer.ViewModel.Editing.Relation;
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
        void NotifyElementsReportReady(ElementListViewModel report);
        void NotifyRelationsReportReady(RelationListViewModel report);

        event EventHandler<ElementCreateViewModel> ElementCreateStarted;
        event EventHandler<ElementEditNameViewModel> ElementEditNameStarted;
        event EventHandler<ElementEditTypeViewModel> ElementEditTypeStarted;

        event EventHandler<RelationCreateViewModel> RelationCreateStarted;
        event EventHandler<RelationEditWeightViewModel> RelationEditWeightStarted;
        event EventHandler<RelationEditTypeViewModel> RelationEditTypeStarted;

        event EventHandler<ReportViewModel> ReportCreated;
        event EventHandler<ElementListViewModel> ElementsReportReady;
        event EventHandler<RelationListViewModel> RelationsReportReady;

        event EventHandler<ActionListViewModel> ActionsVisible;

        event EventHandler<SettingsViewModel> SettingsVisible;

        ICommand ToggleElementExpandedCommand { get; }
        ICommand MoveUpElementCommand { get; }
        ICommand MoveDownElementCommand { get; }
        ICommand SortElementCommand { get; }
        ICommand ShowElementDetailMatrixCommand { get; }
        ICommand ShowElementContextMatrixCommand { get; }
        ICommand ShowCellDetailMatrixCommand { get; }

        ICommand CreateElementCommand { get; }
        ICommand DeleteElementCommand { get; }
        ICommand ChangeElementParentCommand { get; }
        ICommand ChangeElementNameCommand { get; }
        ICommand ChangeElementTypeCommand { get; }
        ICommand CreateRelationCommand { get; }
        ICommand ChangeRelationWeightCommand { get; }
        ICommand ChangeRelationTypeCommand { get; }
        ICommand DeleteRelationCommand { get; }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.ViewModel.Main
{
    public interface IMainViewModel : INotifyPropertyChanged
    {
        void NotifyElementsReportReady(string title, IEnumerable<IDsmElement> elements);
        void NotifyRelationsReportReady(string title, IEnumerable<IDsmRelation> relations);

        ICommand ToggleElementExpandedCommand { get; }
        ICommand MoveUpElementCommand { get; }
        ICommand MoveDownElementCommand { get; }

        ICommand BookmarkElementCommand { get; }
        ICommand AnnotateElementCommand { get; }

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

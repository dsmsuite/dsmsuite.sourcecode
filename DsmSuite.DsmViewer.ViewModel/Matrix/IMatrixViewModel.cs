using System.Windows.Input;

namespace DsmSuite.DsmViewer.ViewModel.Matrix
{
    public interface IMatrixViewModel
    {
        ICommand ToggleElementExpandedCommand { get; }
        ICommand SortElementCommand { get; }
        ICommand MoveUpElementCommand { get; }
        ICommand MoveDownElementCommand { get; }

        ICommand ToggleElementBookmarkCommand { get; }
        ICommand ChangeElementAnnotationCommand { get; }

        ICommand ChangeElementParentCommand { get; }
    }
}

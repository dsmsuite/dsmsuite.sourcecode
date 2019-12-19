using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Lists
{
    public class ActionListItemViewModel : ViewModelBase
    {
        public ActionListItemViewModel(int index, IAction action)
        {
            Index = index;
            Action = action.Title;
            Details = action.Details;
        }

        public int Index { get; }
        public string Action { get; }
        public string Details { get; }
    }
}

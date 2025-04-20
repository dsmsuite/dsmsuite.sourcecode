using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Management
{
    public interface IActionContext
    {
        void AddElementToClipboard(IDsmElement element);
        void RemoveElementFromClipboard(IDsmElement element);
        IDsmElement GetElementOnClipboard();
        bool IsElementOnClipboard();
    }
}

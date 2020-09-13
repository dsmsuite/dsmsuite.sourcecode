using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmElementModelFileCallback
    {
        IDsmElement FindElementById(int elementId);
        IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parent, bool deleted);
        IDsmElement GetRootElement();
        int GetExportedElementCount();
    }
}

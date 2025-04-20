using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmActionModelFileCallback
    {
        IDsmAction ImportAction(int id, string type, IReadOnlyDictionary<string, string> data);
        IEnumerable<IDsmAction> GetExportedActions();
        int GetExportedActionCount();
    }
}

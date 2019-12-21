using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmElementModelFileCallback
    {
        IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parent);
        IEnumerable<IDsmElement> GetExportedRootElements();
        int GetExportedElementCount();
    }
}

using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public interface IDsmElementModelFileCallback
    {
        IDsmElement FindElementById(int elementId);
        IDsmElement ImportElement(int id, string name, string type, IDictionary<string, string> properties, int order, bool expanded, int? parent, bool deleted);
        IDsmElement GetRootElement();
        int GetExportedElementCount();
    }
}

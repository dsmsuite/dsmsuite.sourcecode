using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiElementModelFileCallback
    {
        IDsiElement ImportElement(int id, string name, string type, string source);
        IEnumerable<IDsiElement> GetExportedElements();
        int GetExportedElementCount();
    }
}

using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public interface IDsiElementModelFileCallback
    {
        IDsiElement ImportElement(int id, string name, string type, IDictionary<string, string> properties);
        IEnumerable<IDsiElement> GetElements();
        int CurrentElementCount { get; }
    }
}

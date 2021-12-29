using System.Collections.Generic;

namespace DsmSuite.Analyzer.Model.Interface
{
    public interface IDsiElement
    {
        int Id { get; }
        string Name { get; }
        string Type { get; }
        IDictionary<string,string> Properties { get; }
    }
}

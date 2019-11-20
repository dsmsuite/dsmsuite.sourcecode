using System.Collections.Generic;

namespace DsmSuite.Analyzer.Data
{
    public interface IElement
    {
        int ElementId { get; }
        string Name { get; }
        string Type { get; }
        string Source { get; }
        ICollection<IRelation> Providers { get; }
    }
}

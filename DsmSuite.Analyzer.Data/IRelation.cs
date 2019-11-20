
namespace DsmSuite.Analyzer.Data
{
    public interface IRelation
    {
        IElement Consumer { get; }
        IElement Provider { get; }
        string Type { get; }
        int Weight { get; }
    }
}

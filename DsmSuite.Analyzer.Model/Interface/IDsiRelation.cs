
namespace DsmSuite.Analyzer.Model.Interface
{
    public interface IDsiRelation
    {
        int Consumer{ get; }
        int Provider { get; }
        string Type { get; }
        int Weight { get; }
    }
}


namespace DsmSuite.Analyzer.Model.Interface
{
    public interface IRelation
    {
        int ConsumerId{ get; }
        int ProviderId { get; }
        string Type { get; }
        int Weight { get; }
    }
}

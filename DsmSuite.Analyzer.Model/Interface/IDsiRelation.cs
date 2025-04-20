namespace DsmSuite.Analyzer.Model.Interface
{
    public interface IDsiRelation
    {
        int ConsumerId { get; }
        int ProviderId { get; }
        string Type { get; }
        int Weight { get; }
        IDictionary<string, string> Properties { get; }
    }
}

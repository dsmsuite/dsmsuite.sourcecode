namespace DsmSuite.Analyzer.Model.Interface
{
    public interface IElement
    {
        int ElementId { get; }
        string Name { get; }
        string Type { get; }
        string Source { get; }
        //ICollection<IRelation> Providers { get; }
    }
}

namespace DsmSuite.Analyzer.Model.Interface
{
    public interface IElement
    {
        int Id { get; }
        string Name { get; }
        string Type { get; }
        string Source { get; }
        //ICollection<IRelation> Providers { get; }
    }
}

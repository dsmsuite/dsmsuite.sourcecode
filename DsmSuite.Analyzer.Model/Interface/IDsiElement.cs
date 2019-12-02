namespace DsmSuite.Analyzer.Model.Interface
{
    public interface IDsiElement
    {
        int Id { get; }
        string Name { get; }
        string Type { get; }
        string Source { get; }
    }
}

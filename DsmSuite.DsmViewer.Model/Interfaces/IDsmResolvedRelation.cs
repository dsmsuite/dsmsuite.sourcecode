namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmResolvedRelation
    {
        int Id { get; }
        IDsmElement Consumer { get; }
        IDsmElement Provider { get; }
        string Type { get; }
        int Weight { get; }
    }
}

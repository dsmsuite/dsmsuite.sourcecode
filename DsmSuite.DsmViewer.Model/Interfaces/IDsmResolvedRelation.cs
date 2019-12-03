namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmResolvedRelation
    {
        IDsmElement Consumer { get; }
        IDsmElement Provider { get; }
        string Type { get; }
        int Weight { get; }
    }
}

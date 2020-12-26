namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmRelationAnnotation
    {
        int ConsumerId { get; }
        int ProviderId { get; }
        string Text { get; set; }
    }
}

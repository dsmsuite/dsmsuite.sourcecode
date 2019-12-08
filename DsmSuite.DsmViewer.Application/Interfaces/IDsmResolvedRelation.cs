using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public interface IDsmResolvedRelation
    {
        int Id { get; }
        IDsmElement ConsumerElement { get; }
        IDsmElement ProviderElement { get; }
        string Type { get; }
        int Weight { get; }
    }
}

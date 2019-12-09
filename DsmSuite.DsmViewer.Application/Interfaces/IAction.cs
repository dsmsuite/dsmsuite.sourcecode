namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public interface IAction
    {
        string Type { get; }
        string Details { get; }
        string Description { get; }
    }
}

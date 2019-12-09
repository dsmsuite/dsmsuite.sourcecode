namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public interface IAction
    {
        string Type { get; }
        string Details { get; }
        string Description { get; }
    }
}

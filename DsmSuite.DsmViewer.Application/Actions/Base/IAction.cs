namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public interface IAction
    {
        void Do();
        void Undo();

        string Description { get; }
    }
}

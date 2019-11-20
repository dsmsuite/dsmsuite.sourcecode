namespace DsmSuite.DsmViewer.Model.Actions.Base
{
    public interface IAction
    {
        void Do();
        void Undo();

        string Description { get; }
    }
}

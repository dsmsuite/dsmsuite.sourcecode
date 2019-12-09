using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public abstract class ActionBase : IAction
    {
        protected ActionBase(IDsmModel model)
        {
            Model = model;
        }

        protected IDsmModel Model { get; }

        public abstract void Do();

        public abstract void Undo();

        public abstract string Type { get; }
        public abstract string Details { get; }

        public string Description => $"{Type} : {Details}";
    }
}

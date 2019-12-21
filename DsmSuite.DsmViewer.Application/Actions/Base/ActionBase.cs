using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Base
{
    public abstract class ActionBase : IAction
    {
        protected ActionBase(IDsmModel model)
        {
            Model = model;
        }

        protected IDsmModel Model { get; }
        public abstract string ActionName { get; }
        public abstract string Title { get; }
        public abstract string Description { get; }

        public abstract void Do();

        public abstract void Undo();

        public abstract IReadOnlyDictionary<string, string> Pack();
    }
}

using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class SnapshotAction : ActionBase, IAction
    {
        public SnapshotAction(IDsmModel model) : base(model)
        {
        }

        public void Do()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string Description => "Create element";
    }
}

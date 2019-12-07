using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class SnapshotAction : ActionBase
    {
        public SnapshotAction(IDsmModel model) : base(model)
        {
        }

        public override void Do()
        {
            throw new NotImplementedException();
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override string Description => "Snapshot";
    }
}

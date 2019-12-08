using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class SnapshotAction : ActionBase
    {
        private readonly string _snapshotText;

        public SnapshotAction(IDsmModel model, string snapshotText) : base(model)
        {
            _snapshotText = snapshotText;
        }

        public override void Do()
        {
        }

        public override void Undo()
        {
        }

        public override string Description => $"Snapshot {_snapshotText}";
    }
}

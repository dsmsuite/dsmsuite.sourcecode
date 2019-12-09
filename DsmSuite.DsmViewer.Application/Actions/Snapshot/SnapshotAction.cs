using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class SnapshotAction : ActionBase
    {
        private readonly string _description;

        public SnapshotAction(IDsmModel model, string description) : base(model)
        {
            _description = description;
        }

        public override void Do()
        {
        }

        public override void Undo()
        {
        }

        public override string Description => $"Snapshot {_description}";
    }
}

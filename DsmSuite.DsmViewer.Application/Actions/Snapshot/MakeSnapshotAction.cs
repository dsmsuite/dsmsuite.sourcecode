using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class MakeSnapshotAction : ActionBase
    {
        private readonly string _description;

        public MakeSnapshotAction(IDsmModel model, string description) : base(model)
        {
            _description = description;

            Type = "Make snapshot";
            Details = _description;
        }

        public override void Do()
        {
        }

        public override void Undo()
        {
        }
    }
}

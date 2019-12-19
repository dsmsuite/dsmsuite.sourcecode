using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class MakeSnapshotAction : ActionBase
    {
        public string Name { get; }

        public MakeSnapshotAction(IDsmModel model, string name) : base(model)
        {
            Name = name;

            ClassName = nameof(MakeSnapshotAction);
            Title = "Make snapshot";
            Details = name;
        }

        public override void Do()
        {
        }

        public override void Undo()
        {
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            return null;
        }

        public override IAction Unpack(IReadOnlyDictionary<string, string> data)
        {
            return null;
        }
    }
}

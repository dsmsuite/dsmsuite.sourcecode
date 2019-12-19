using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class MakeSnapshotAction : ActionBase
    {
        private readonly string _name;

        public MakeSnapshotAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            _name = GetString(data, nameof(_name));
        }

        public MakeSnapshotAction(IDsmModel model, string name) : base(model)
        {
            _name = name;
        }

        public override string ActionName => nameof(MakeSnapshotAction);
        public override string Title => "Make snapshot";
        public override string Description => $"name={_name}";

        public override void Do()
        {
        }

        public override void Undo()
        {
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetString(data, nameof(_name), _name);
            return data;
        }
    }
}

using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class MakeSnapshotAction : IAction
    {
        private readonly string _name;

        public const string TypeName = "snapshot";

        public MakeSnapshotAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            _name = attributes.GetString(nameof(_name));
        }

        public MakeSnapshotAction(string name)
        {
            _name = name;
        }
        
        public string Type => TypeName;
        public string Title => "Make snapshot";
        public string Description => $"name={_name}";

        public void Do()
        {
        }

        public void Undo()
        {
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetString(nameof(_name), _name);
                return attributes.Data;
            }
        }
    }
}

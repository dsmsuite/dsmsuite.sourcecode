using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Snapshot
{
    public class MakeSnapshotAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IActionContext _actionContext;
        private readonly string _name;

        public const ActionType RegisteredType = ActionType.Snapshot;

        public MakeSnapshotAction(IDsmModel model, string name)
        {
            _name = name;
        }

        public ActionType Type => RegisteredType;
        public string Title => "Make snapshot";
        public string Description => $"name={_name}";

        public object Do()
        {
            return null;
        }

        public void Undo()
        {
        }

        public bool IsValid()
        {
            return (_model != null) &&
                   (_name != null);
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

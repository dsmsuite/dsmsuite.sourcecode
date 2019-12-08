using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementRenameAction : ActionBase
    {
        private readonly int _elementId;
        private readonly string _oldName;
        private readonly string _newName;

        public ElementRenameAction(IDsmModel model, IDsmElement element, string newName) : base(model)
        {
            _elementId = element.Id;
            _oldName = element.Name;
            _newName = newName;
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element != null)
            {
                element.Name = _newName;
            }
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element != null)
            {
                element.Name = _oldName;
            }
        }

        public override string Description => "Rename element";
    }
}

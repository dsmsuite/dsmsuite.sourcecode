using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementChangeNameAction : ActionBase
    {
        private readonly IDsmElement _element;
        private readonly string _oldName;
        private readonly string _newName;

        public ElementChangeNameAction(IDsmModel model, IDsmElement element, string newName) : base(model)
        {
            _element = element;
            _oldName = _element.Name;
            _newName = newName;
        }

        public override void Do()
        {
            _element.Name = _newName;
        }

        public override void Undo()
        {
            _element.Name = _oldName;
        }

        public override string Description => "Rename element";
    }
}

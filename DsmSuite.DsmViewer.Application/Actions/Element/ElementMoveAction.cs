using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveAction : ActionBase
    {
        private readonly IDsmElement _element;
        private readonly IDsmElement _oldParent;
        private readonly IDsmElement _newParent;

        public ElementMoveAction(IDsmModel model, IDsmElement element, IDsmElement newParent) : base(model)
        {
            _element = element;
            _oldParent = _element.Parent;
            _newParent = newParent;
        }

        public override void Do()
        {
            Model.ChangeParent(_element, _newParent);
        }

        public override void Undo()
        {
            Model.ChangeParent(_element, _oldParent);
        }

        public override string Type => "Move element";
        public override string Details => "element={_element.Fullname} parent={_oldParent.Fullname} -> {_newParent.Fullname}";
    }
}

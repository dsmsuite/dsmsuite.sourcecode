using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveAction : ActionBase
    {
        private readonly int _elementId;
        private readonly int _oldParentId;
        private readonly int _newParentId;

        public ElementMoveAction(IDsmModel model, IDsmElement element, IDsmElement newParent) : base(model)
        {
            _elementId = element.Id;
            _oldParentId = element.Parent.Id;
            _newParentId = newParent.Id;

            Type = "Move element";
            Details = $"element={element.Fullname} parent={element.Parent.Fullname} -> {newParent.Fullname}";
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            IDsmElement newParent = Model.GetElementById(_newParentId);
            if ((element != null) && (newParent != null))
            {
                Model.ChangeParent(element, newParent);
            }
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            IDsmElement oldParent = Model.GetElementById(_oldParentId);
            if ((element != null) && (oldParent != null))
            {
                Model.ChangeParent(element, oldParent);
            }
        }
    }
}

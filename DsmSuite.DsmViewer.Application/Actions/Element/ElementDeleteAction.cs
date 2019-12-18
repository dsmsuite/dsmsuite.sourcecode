using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementDeleteAction : ActionBase
    {
        private readonly int _elementId;

        public ElementDeleteAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _elementId = element.Id;
            Type = "Delete element";
            Details = $"element={element.Fullname}";
        }

        public override void Do()
        {
            Model.RemoveElement(_elementId);
        }

        public override void Undo()
        {
            Model.UnremoveElement(_elementId);
        }
    }
}

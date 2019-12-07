using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementDeleteAction : ActionBase
    {
        private readonly int _elementId;

        public ElementDeleteAction(IDsmModel model, int elementId) : base(model)
        {
            _elementId = elementId;
        }

        public override void Do()
        {
            Model.RemoveElement(_elementId);
        }

        public override void Undo()
        {
            Model.RestoreElement(_elementId);
        }

        public override string Description => "Delete element";
    }
}

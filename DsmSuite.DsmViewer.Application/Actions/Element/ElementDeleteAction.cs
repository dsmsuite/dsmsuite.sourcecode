using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Actions.Element
{
    public class ElementDeleteAction : ActionBase, IAction
    {
        private int _elementId;

        public ElementDeleteAction(IDsmModel model, int elementId) : base(model)
        {
            _elementId = elementId;
        }

        public void Do()
        {
            Model.RemoveElement(_elementId);
        }

        public void Undo()
        {
            Model.RestoreElement(_elementId);
        }

        public string Description => "Delete element";
    }
}

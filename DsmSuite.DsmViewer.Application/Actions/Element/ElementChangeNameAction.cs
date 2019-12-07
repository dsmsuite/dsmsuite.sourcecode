using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementChangeNameAction : ActionBase
    {
        private readonly int _elementId;
        private readonly string _oldName;
        private readonly string _newName;

        public ElementChangeNameAction(IDsmModel model, int elementId, string oldName, string newName) : base(model)
        {
            _elementId = elementId;
            _oldName = oldName;
            _newName = newName;
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element.Name == _oldName)
            {
                element.Name = _newName;
            }
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element.Name == _newName)
            {
                element.Name = _oldName;
            }
        }

        public override string Description => "Rename element";
    }
}

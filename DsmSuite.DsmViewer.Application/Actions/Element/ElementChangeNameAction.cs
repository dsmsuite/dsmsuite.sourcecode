using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementChangeNameAction : ActionBase, IAction
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

        public void Do()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element.Name == _oldName)
            {
                element.Name = _newName;
            }
        }

        public void Undo()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element.Name == _newName)
            {
                element.Name = _oldName;
            }
        }

        public string Description => "Rename element";
    }
}

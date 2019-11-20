using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Actions.Base;

namespace DsmSuite.DsmViewer.Model.Actions.Element
{
    public class ElementChangeNameAction : ActionBase, IAction
    {
        private int _elementId;
        private string _oldName;
        private string _newName;

        public ElementChangeNameAction(IDsmModel model, int elementId, string oldName, string newName) : base(model)
        {
            _elementId = elementId;
            _oldName = oldName;
            _newName = newName;
        }

        public void Do()
        {
            IElement element = Model.GetElementById(_elementId);
            if (element.Name == _oldName)
            {
                element.Name = _newName;
            }
        }

        public void Undo()
        {
            IElement element = Model.GetElementById(_elementId);
            if (element.Name == _newName)
            {
                element.Name = _oldName;
            }
        }

        public string Description => "Rename element";
    }
}

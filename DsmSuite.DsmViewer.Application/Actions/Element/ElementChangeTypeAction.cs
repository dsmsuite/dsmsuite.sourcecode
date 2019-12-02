using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Actions.Element
{
    public class ElementChangeTypeAction : ActionBase, IAction
    {
        private int _elementId;
        private string _oldType;
        private string _newType;

        public ElementChangeTypeAction(IDsmModel model, int elementId, string oldType, string newType) : base(model)
        {
            _elementId = elementId;
            _oldType = oldType;
            _newType = newType;
        }

        public void Do()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string Description => "Rename element";
    }
}

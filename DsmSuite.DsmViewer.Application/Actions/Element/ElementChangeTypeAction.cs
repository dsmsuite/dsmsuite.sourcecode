using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementChangeTypeAction : ActionBase
    {
        private readonly int _elementId;
        private readonly string _oldType;
        private readonly string _newType;

        public ElementChangeTypeAction(IDsmModel model, int elementId, string oldType, string newType) : base(model)
        {
            _elementId = elementId;
            _oldType = oldType;
            _newType = newType;
        }

        public override void Do()
        {
            throw new NotImplementedException();
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override string Description => "Rename element";
    }
}

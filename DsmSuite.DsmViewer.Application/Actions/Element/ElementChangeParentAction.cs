using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementChangeParentAction : ActionBase
    {
        public ElementChangeParentAction(IDsmModel model) : base(model)
        {
        }

        public override void Do()
        {
            throw new NotImplementedException();
        }

        public override void Undo()
        {
            throw new NotImplementedException();
        }

        public override string Description => "Move element";
    }
}

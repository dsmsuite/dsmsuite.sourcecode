using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Actions.Element
{
    public class ElementMoveUpAction : ActionBase, IAction
    {
        public ElementMoveUpAction(IDsmModel model) : base(model)
        {
        }

        public void Do()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string Description => "Move down element";
    }
}

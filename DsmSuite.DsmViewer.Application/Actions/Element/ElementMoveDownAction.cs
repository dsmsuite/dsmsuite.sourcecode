using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveUpAction : ActionBase, IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;

        public ElementMoveUpAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _model = model;
            _element = element;
        }

        public void Do()
        {
            IDsmElement previous = _element?.PreviousSibling;
            if ((_element != null) && (previous != null))
            {
                _model.Swap(_element, previous);
            }
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string Description => "Move down element";
    }
}

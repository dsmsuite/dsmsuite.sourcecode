using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveDownAction : ActionBase, IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;

        public ElementMoveDownAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _model = model;
            _element = element;
        }

        public void Do()
        {
            IDsmElement next = _element?.NextSibling;
            if ((_element != null) && (next != null))
            {
                _model.Swap(_element, next);
            }
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string Description => "Move up element";
    }
}

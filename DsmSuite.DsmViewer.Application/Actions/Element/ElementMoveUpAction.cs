using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveDownAction : ActionBase
    {
        private readonly IDsmElement _currentElement;
        private readonly IDsmElement _nextElement;

        public ElementMoveDownAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _currentElement = element;
            _nextElement = element?.NextSibling;
        }

        public override void Do()
        {
            if (_currentElement != null && _nextElement != null)
            {
                Model.Swap(_currentElement, _nextElement);
            }
        }

        public override void Undo()
        {
            if (_currentElement != null && _nextElement != null)
            {
                Model.Swap(_nextElement, _currentElement);
            }
        }

        public override string Description => $"Move up element name={_currentElement.Fullname}";
    }
}

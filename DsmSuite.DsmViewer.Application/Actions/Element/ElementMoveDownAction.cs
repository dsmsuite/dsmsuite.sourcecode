using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveUpAction : ActionBase, IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _currentElement;
        private readonly IDsmElement _previousElement;

        public ElementMoveUpAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _model = model;
            _currentElement = element;
            _previousElement = element?.PreviousSibling;
        }

        public void Do()
        {
            if (_currentElement != null && _previousElement != null)
            {
                _model.Swap(_currentElement, _previousElement);
            }
        }

        public void Undo()
        {
            if (_currentElement != null && _previousElement != null)
            {
                _model.Swap(_previousElement, _currentElement);
            }
        }

        public string Description => $"Move down element name={_currentElement.Fullname}";
    }
}

using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveDownAction : ActionBase, IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _currentElement;
        private readonly IDsmElement _nextElement;

        public ElementMoveDownAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _model = model;
            _currentElement = element;
            _nextElement = element?.NextSibling;
        }

        public void Do()
        {
            if (_currentElement != null && _nextElement != null)
            {
                _model.Swap(_currentElement, _nextElement);
            }
        }

        public void Undo()
        {
            if (_currentElement != null && _nextElement != null)
            {
                _model.Swap(_nextElement, _currentElement);
            }
        }

        public string Description => $"Move up element name={_currentElement.Fullname}";
    }
}

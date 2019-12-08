using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveUpAction : ActionBase
    {
        private readonly IDsmElement _currentElement;
        private readonly IDsmElement _previousElement;

        public ElementMoveUpAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _currentElement = element;
            _previousElement = element?.PreviousSibling;
        }

        public override void Do()
        {
            if (_currentElement != null && _previousElement != null)
            {
                Model.Swap(_currentElement, _previousElement);
            }
        }

        public override void Undo()
        {
            if (_currentElement != null && _previousElement != null)
            {
                Model.Swap(_previousElement, _currentElement);
            }
        }

        public override string Description => $"Move down element name={_currentElement.Fullname}";
    }
}

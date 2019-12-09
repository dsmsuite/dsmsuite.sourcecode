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

        public override string Type => $"Move down element";
        public override string Details => "name={_currentElement.Fullname}";
    }
}

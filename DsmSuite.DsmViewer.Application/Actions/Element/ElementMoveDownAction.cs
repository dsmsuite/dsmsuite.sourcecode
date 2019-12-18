using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveDownAction : ActionBase
    {
        private readonly int _elementId;

        public ElementMoveDownAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _elementId = element.Id;

            Type = $"Move down element";
            Details = $"name={element.Fullname}";
        }

        public override void Do()
        {
            IDsmElement currentElement = Model.GetElementById(_elementId);
            IDsmElement nextElement = currentElement?.NextSibling;
            if (currentElement != null && nextElement != null)
            {
                Model.Swap(currentElement, nextElement);
            }
        }

        public override void Undo()
        {
            IDsmElement currentElement = Model.GetElementById(_elementId);
            IDsmElement previousElement = currentElement?.PreviousSibling;
            if (currentElement != null && previousElement != null)
            {
                Model.Swap(previousElement, currentElement);
            }
        }
    }
}

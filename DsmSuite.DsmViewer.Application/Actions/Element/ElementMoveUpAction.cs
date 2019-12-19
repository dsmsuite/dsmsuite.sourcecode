using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveUpAction : ActionBase
    {
        public int Element { get; }

        public ElementMoveUpAction(IDsmModel model, IDsmElement element) : base(model)
        {
            Element = element.Id;

            ClassName = nameof(ElementMoveUpAction);
            Title = $"Move up element";
            Details = $"name={element.Fullname}";
        }

        public override void Do()
        {
            IDsmElement currentElement = Model.GetElementById(Element);
            Debug.Assert(currentElement != null);

            IDsmElement previousElement = currentElement?.PreviousSibling;
            Debug.Assert(previousElement != null);

            Model.Swap(currentElement, previousElement);
        }

        public override void Undo()
        {
            IDsmElement currentElement = Model.GetElementById(Element);
            Debug.Assert(currentElement != null);

            IDsmElement nextElement = currentElement?.NextSibling;
            Debug.Assert(nextElement != null);

            Model.Swap(currentElement, nextElement);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            return null;
        }

        public override IAction Unpack(IReadOnlyDictionary<string, string> data)
        {
            return null;
        }
    }
}

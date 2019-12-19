using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementDeleteAction : ActionBase
    {
        public int Element { get; private set; }

        public ElementDeleteAction(IDsmModel model, IDsmElement element) : base(model)
        {
            Element = element.Id;

            ClassName = nameof(ElementDeleteAction);
            Title = "Delete element";
            Details = $"element={element.Fullname}";
        }

        public override void Do()
        {
            Model.RemoveElement(Element);
        }

        public override void Undo()
        {
            Model.UnremoveElement(Element);
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

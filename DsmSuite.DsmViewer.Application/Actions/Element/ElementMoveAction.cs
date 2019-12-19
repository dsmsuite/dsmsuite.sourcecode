using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveAction : ActionBase
    {
        public int Element { get; }
        public int OldParent { get; }
        public int NewParent { get; }

        public ElementMoveAction(IDsmModel model, IDsmElement element, IDsmElement newParent) : base(model)
        {
            Element = element.Id;
            OldParent = element.Parent.Id;
            NewParent = newParent.Id;

            ClassName = nameof(ElementMoveAction);
            Title = "Move element";
            Details = $"element={element.Fullname} parent={element.Parent.Fullname} -> {newParent.Fullname}";
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(Element);
            Debug.Assert(element != null);

            IDsmElement newParent = Model.GetElementById(NewParent);
            Debug.Assert(newParent != null);

            Model.ChangeParent(element, newParent);
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(Element);
            Debug.Assert(element != null);

            IDsmElement oldParent = Model.GetElementById(OldParent);
            Debug.Assert(oldParent != null);

            Model.ChangeParent(element, oldParent);
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

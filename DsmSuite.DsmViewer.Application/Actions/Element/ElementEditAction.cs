using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementEditAction : ActionBase
    {
        public int Element { get; private set; }
        public string OldName { get; }
        public string NewName { get; }
        public string OldType { get; }
        public string NewType { get; }

        public ElementEditAction(IDsmModel model, IDsmElement element, string name, string type) : base(model)
        {
            Element = element.Id;
            OldName = element.Name;
            NewName = name;
            OldType = element.Type;
            NewType = type;

            ClassName = nameof(ElementEditAction);
            Title = "Edit element";
            Details = $"element={element.Fullname} name={OldName} -> {NewName}  type={OldType} -> {NewType}";
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(Element);
            Debug.Assert(element != null);

            Model.EditElement(element, NewName, NewType);
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(Element);
            Debug.Assert(element != null);

            Model.EditElement(element, OldName, NewType);
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

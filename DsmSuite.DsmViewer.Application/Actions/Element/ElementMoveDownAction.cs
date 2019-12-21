using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveDownAction : ActionBase
    {
        private readonly IDsmElement _element;

        public ElementMoveDownAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);
        }

        public ElementMoveDownAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _element = element;
            Debug.Assert(_element != null);
        }

        public override string ActionName => nameof(ElementMoveDownAction);
        public override string Title => "Move down element";
        public override string Description => $"element={_element.Fullname}";

        public override void Do()
        {
            IDsmElement nextElement = Model.NextSibling(_element);
            Debug.Assert(nextElement != null);

            Model.Swap(_element, nextElement);
        }

        public override void Undo()
        {
            IDsmElement previousElement = Model.PreviousSibling(_element);
            Debug.Assert(previousElement != null);

            Model.Swap(previousElement, _element);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            ActionAttributes attributes = new ActionAttributes();
            attributes.SetInt(nameof(_element), _element.Id);
            return attributes.GetData();
        }
    }
}

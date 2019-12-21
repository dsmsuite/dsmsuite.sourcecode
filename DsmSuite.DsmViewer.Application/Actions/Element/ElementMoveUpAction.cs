using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveUpAction : ActionBase
    {
        private readonly IDsmElement _element;

        public ElementMoveUpAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            int id = GetInt(data, nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);
        }

        public ElementMoveUpAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _element = element;
            Debug.Assert(_element != null);
        }

        public override string ActionName => nameof(ElementMoveUpAction);
        public override string Title => "Move up element";
        public override string Description => $"element={_element.Fullname}";

        public override void Do()
        {
            IDsmElement previousElement = Model.PreviousSibling(_element);
            Debug.Assert(previousElement != null);

            Model.Swap(_element, previousElement);
        }

        public override void Undo()
        {
            IDsmElement nextElement = Model.NextSibling(_element);
            Debug.Assert(nextElement != null);

            Model.Swap(_element, nextElement);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_element), _element.Id);
            return data;
        }
    }
}

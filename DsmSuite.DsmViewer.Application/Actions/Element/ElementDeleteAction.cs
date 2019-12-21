using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementDeleteAction : ActionBase
    {
        private readonly IDsmElement _element;

        public ElementDeleteAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetDeletedElementById(id);
            Debug.Assert(_element != null);
        }

        public ElementDeleteAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _element = element;
            Debug.Assert(_element != null);
        }

        public override string ActionName => nameof(ElementDeleteAction);
        public override string Title => "Delete element";
        public override string Description => $"element={_element.Fullname}";

        public override void Do()
        {
            Model.RemoveElement(_element.Id);
        }

        public override void Undo()
        {
            Model.UnremoveElement(_element.Id);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            ActionAttributes attributes = new ActionAttributes();
            attributes.SetInt(nameof(_element), _element.Id);
            return attributes.GetData();
        }
    }
}

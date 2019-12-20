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
            int id = GetInt(data, nameof(_element));
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
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_element), _element.Id);
            return data;
        }
    }
}

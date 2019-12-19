using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementEditNameAction : ActionBase
    {
        private readonly IDsmElement _element;
        private readonly string _old;
        private readonly string _new;

        public ElementEditNameAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            int id = GetInt(data, nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _old = GetString(data, nameof(_old));
            _new = GetString(data, nameof(_new));
        }

        public ElementEditNameAction(IDsmModel model, IDsmElement element, string name) : base(model)
        {
            _element = element;
            Debug.Assert(_element != null);

            _old = _element.Name;
            _new = name;
        }

        public override string ActionName => nameof(ElementEditNameAction);
        public override string Title => "Edit element name";
        public override string Description => $"element={_element.Fullname} name={_old}->{_new}";

        public override void Do()
        {
            Model.EditElement(_element, _new, _element.Type);
        }

        public override void Undo()
        {
            Model.EditElement(_element, _old, _element.Type);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_element), _element.Id);
            SetString(data, nameof(_old), _old);
            SetString(data, nameof(_new), _new);
            return data;
        }
    }
}

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
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _old = attributes.GetString(nameof(_old));
            _new = attributes.GetString(nameof(_new));
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
            Model.EditElementName(_element, _new);
        }

        public override void Undo()
        {
            Model.EditElementName(_element, _old);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            ActionAttributes attributes = new ActionAttributes();
            attributes.SetInt(nameof(_element), _element.Id);
            attributes.SetString(nameof(_old), _old);
            attributes.SetString(nameof(_new), _new);
            return attributes.GetData();
        }
    }
}

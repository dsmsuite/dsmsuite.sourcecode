using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementCreateAction : ActionBase
    {
        private IDsmElement _element;
        private readonly string _name;
        private readonly string _type;
        private readonly IDsmElement _parent;

        public ElementCreateAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _name = attributes.GetString(nameof(_name));
            _type = attributes.GetString(nameof(_type));

            int? parentId = attributes.GetNullableInt(nameof(_parent));
            if (parentId.HasValue)
            {
                _parent = model.GetElementById(parentId.Value);
            }
        }

        public ElementCreateAction(IDsmModel model, string name, string type, IDsmElement parent) : base(model)
        {
            _name = name;
            _type = type;
            _parent = parent;
        }

        public override string ActionName => nameof(ElementCreateAction);
        public override string Title => "Create element";
        public override string Description => $"name={_name} type={_type} parent={_parent.Fullname}";

        public override void Do()
        {
            _element = Model.AddElement(_name, _type, _parent.Id);
            Debug.Assert(_element != null);
        }

        public override void Undo()
        {
            Model.RemoveElement(_element.Id);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            ActionAttributes attributes = new ActionAttributes();
            attributes.SetInt(nameof(_element), _element.Id);
            attributes.SetString(nameof(_name), _name);
            attributes.SetString(nameof(_type), _type);
            attributes.SetNullableInt(nameof(_parent), _parent.Id);
            return attributes.GetData();
        }
    }
}

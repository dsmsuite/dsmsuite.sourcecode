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
            int id = GetInt(data, nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _name = GetString(data, nameof(_name));
            _type = GetString(data, nameof(_type));

            int? parentId = GetNullableInt( data, nameof(_parent));
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
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_element), _element.Id);
            SetString(data, nameof(_name), _name);
            SetString(data, nameof(_type), _type);
            SetNullableInt(data, nameof(_parent), _parent.Id);
            return data;
        }
    }
}

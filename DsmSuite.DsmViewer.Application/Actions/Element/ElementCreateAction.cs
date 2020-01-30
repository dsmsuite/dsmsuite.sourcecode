using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementCreateAction : IAction
    {
        private readonly IDsmModel _model;
        private IDsmElement _element;
        private readonly string _name;
        private readonly string _type;
        private readonly IDsmElement _parent;

        public const string TypeName = "ecreate";

        public ElementCreateAction(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            IReadOnlyDictionary<string, string> data = args[1] as IReadOnlyDictionary<string, string>;
            Debug.Assert(data != null);

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(_model, data);
            _element = attributes.GetElement(nameof(_element));
            Debug.Assert(_element != null);

            _name = attributes.GetString(nameof(_name));
            _type = attributes.GetString(nameof(_type));

            int? parentId = attributes.GetNullableInt(nameof(_parent));
            if (parentId.HasValue)
            {
                _parent = _model.GetElementById(parentId.Value);
            }
        }

        public ElementCreateAction(IDsmModel model, string name, string type, IDsmElement parent)
        {
            _model = model;
            Debug.Assert(_model != null);

            _name = name;
            _type = type;
            _parent = parent;
        }

        public string Type => TypeName;
        public string Title => "Create element";
        public string Description => $"name={_name} type={_type} parent={_parent.Fullname}";

        public object Do()
        {
            _element = _model.AddElement(_name, _type, _parent.Id);
            Debug.Assert(_element != null);

            _model.AssignElementOrder();

            return _element;
        }

        public void Undo()
        {
            _model.RemoveElement(_element.Id);
            _model.AssignElementOrder();
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetString(nameof(_name), _name);
                attributes.SetString(nameof(_type), _type);
                attributes.SetNullableInt(nameof(_parent), _parent.Id);
                return attributes.Data;
            }
        }
    }
}

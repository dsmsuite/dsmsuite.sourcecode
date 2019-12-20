using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementDeleteAction : ActionBase
    {
        private readonly int _id;
        private readonly string _name;
        private readonly string _type;
        private readonly IDsmElement _parent;

        public ElementDeleteAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            _id = GetInt(data, nameof(_id));
            _name = GetString(data, nameof(_name));
            _type = GetString(data, nameof(_type));
            int? parentId = GetNullableInt(data, nameof(_parent));
            if (parentId.HasValue)
            {
                _parent = model.GetElementById(parentId.Value);
            }
        }

        public ElementDeleteAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _id = element.Id;
            _name = element.Name;
            _type = element.Type;
            _parent = element.Parent;
        }

        public override string ActionName => nameof(ElementDeleteAction);
        public override string Title => "Delete element";
        public override string Description => $"element={_name} parent={_parent.Fullname}";

        public override void Do()
        {
            Model.RemoveElement(_id);
        }

        public override void Undo()
        {
            Model.ImportElement(_id, _name, _type, 0, false, _parent?.Id);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_id), _id);
            SetString(data, nameof(_name), _name);
            SetString(data, nameof(_type), _type);
            SetNullableInt(data, nameof(_parent), _parent?.Id);
            return data;
        }
    }
}

using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementCreateAction : ActionBase
    {
        public int Element { get; private set; }
        public string Name { get; }
        public string Type { get; }
        public int? Parent { get; }

        private ElementCreateAction(IDsmModel model, int id, string name, string type, int? parentId) : base(model)
        {
            Element = id;
            Name = name;
            Type = type;
            Parent = parentId;

            ClassName = nameof(ElementCreateAction);
            base.Title = "Create element";
            Details = $"name={name} parent={parentId} type={type}";
        }

        public ElementCreateAction(IDsmModel model, string name, string type, IDsmElement parent) : base(model)
        {
            Name = name;
            Type = type;
            Parent = parent?.Id;

            ClassName = nameof(ElementCreateAction);
            base.Title = "Create element";
            Details = $"name={name} parent={parent.Fullname} type={type}";
        }

        public override void Do()
        {
            IDsmElement element = Model.AddElement(Title, Type, Parent);
            Debug.Assert(element != null);

            Element = element.Id;
        }

        public override void Undo()
        {
            Model.RemoveElement(Element);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            SetInt(data, nameof(Element), Element);
            SetString(data, nameof(Name), Name);
            SetString(data, nameof(Type), Type);
            SetInt(data, nameof(Parent), Parent);

            return data;
        }

        public override IAction Unpack(IReadOnlyDictionary<string, string> data)
        {
            int? id = GetInt(data, nameof(Element));
            string name = GetString(data, nameof(Name));
            string type = GetString(data, nameof(Type));
            int? parentId = GetInt(data, nameof(Parent));

            return new ElementCreateAction(Model, id.Value, name, type, parentId);
        }
    }
}

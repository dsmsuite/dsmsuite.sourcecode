using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementCreateAction : ActionBase
    {
        private int? _elementId;
        private readonly string _name;
        private readonly string _type;
        private readonly IDsmElement _parent;
        
        public ElementCreateAction(IDsmModel model, string name, string type, IDsmElement parent) : base(model)
        {
            _name = name;
            _type = type;
            _parent = parent;

            Type = "Create element";
            Details = $"name={_name} parent={_parent.Fullname} type={_type}";
        }

        public override void Do()
        {
            IDsmElement element = Model.AddElement(_name, _type, _parent.Id);
            _elementId = element?.Id;
        }

        public override void Undo()
        {
            if (_elementId.HasValue)
            {
                Model.RemoveElement(_elementId.Value);
            }
        }
    }
}

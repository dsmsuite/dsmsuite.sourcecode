using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementCreateAction : ActionBase
    {
        private IDsmElement _element;
        private readonly string _name;
        private readonly string _type;
        private readonly IDsmElement _parent;
        
        public ElementCreateAction(IDsmModel model, string name, string type, IDsmElement parent) : base(model)
        {
            _name = name;
            _type = type;
            _parent = parent;
        }

        public override void Do()
        {
            _element = Model.AddElement(_name, _type, _parent.Id);
        }

        public override void Undo()
        {
            Model.RemoveElement(_element.Id);
        }

        public override string Type => "Create element";
        public override string Details => "name={_name} parent={_parent.Fullname} type={_type}";
    }
}

using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementCreateAction : ActionBase
    {
        private int _elementId;
        private readonly string _name;
        private readonly string _type;
        private readonly int _parentId;
        
        public ElementCreateAction(IDsmModel model, string name, string type, int parentId) : base(model)
        {
            _name = name;
            _type = type;
            _parentId = parentId;
        }

        public override void Do()
        {
            IDsmElement element = Model.CreateElement(_name, _type, _parentId);
            _elementId = element.Id;
        }

        public override void Undo()
        {
            Model.RestoreElement(_elementId);
        }

        public override string Description => "Create element";
    }
}

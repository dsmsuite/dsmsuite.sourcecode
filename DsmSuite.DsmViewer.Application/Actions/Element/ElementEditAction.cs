using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementEditAction : ActionBase
    {
        private readonly IDsmElement _element;
        private readonly string _oldName;
        private readonly string _newName;
        private readonly string _oldType;
        private readonly string _newType;

        public ElementEditAction(IDsmModel model, IDsmElement element, string name, string type) : base(model)
        {
            _element = element;
            _oldName = element.Name;
            _newName = name;
            _oldType = element.Type;
            _newType = type;
        }

        public override void Do()
        {
            Model.EditElement(_element, _newName, _newType);
        }

        public override void Undo()
        {
            Model.EditElement(_element, _oldName, _oldType);
        }

        public override string Description => "Edit element";
    }
}

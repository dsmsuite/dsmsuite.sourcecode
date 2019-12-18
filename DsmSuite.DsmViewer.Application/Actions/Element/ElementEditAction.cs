using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementEditAction : ActionBase
    {
        private readonly int _elementId;
        private readonly string _oldName;
        private readonly string _newName;
        private readonly string _oldType;
        private readonly string _newType;

        public ElementEditAction(IDsmModel model, IDsmElement element, string name, string type) : base(model)
        {
            _elementId = element.Id;
            _oldName = element.Name;
            _newName = name;
            _oldType = element.Type;
            _newType = type;

            Type = "Edit element";
            Details = $"element={element.Fullname} name={_oldName} -> {_newName}  type={_oldType} -> {_newType}";
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element != null)
            {
                Model.EditElement(element, _newName, _newType);
            }
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element != null)
            {
                Model.EditElement(element, _oldName, _oldType);
            }
        }
    }
}

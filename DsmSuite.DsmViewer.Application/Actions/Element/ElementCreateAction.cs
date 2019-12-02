using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Actions.Element
{
    public class ElementCreateAction : ActionBase, IAction
    {
        private int _elementId;
        private string _name;
        private string _type;
        private int _parentId;
        
        public ElementCreateAction(IDsmModel model, string name, string type, int parentId) : base(model)
        {
            _name = name;
            _type = type;
            _parentId = parentId;
        }

        public void Do()
        {
            IDsmElement element = Model.CreateElement(_name, _type, _parentId);
            _elementId = element.Id;
        }

        public void Undo()
        {
            Model.RestoreElement(_elementId);
        }

        public string Description => "Create element";
    }
}

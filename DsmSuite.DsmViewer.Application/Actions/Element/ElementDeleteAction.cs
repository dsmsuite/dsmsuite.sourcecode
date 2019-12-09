using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementDeleteAction : ActionBase
    {
        private readonly IDsmElement _element;

        public ElementDeleteAction(IDsmModel model, IDsmElement element) : base(model)
        {
            _element = element;
        }

        public override void Do()
        {
            Model.RemoveElement(_element.Id);
        }

        public override void Undo()
        {
            Model.UnremoveElement(_element.Id);
        }

        public override string Type => "Delete element";
        public override string Details => "element={_element.Fullname}";
    }
}

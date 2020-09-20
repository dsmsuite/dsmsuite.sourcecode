using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveDownAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;

        public const ActionType TypeName = ActionType.ElementMoveDown;

        public ElementMoveDownAction(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            IReadOnlyDictionary<string, string> data = args[1] as IReadOnlyDictionary<string, string>;
            Debug.Assert(data != null);

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(_model, data);
            _element = attributes.GetElement(nameof(_element));
            Debug.Assert(_element != null);
        }

        public ElementMoveDownAction(IDsmModel model, IDsmElement element)
        {
            _model = model;
            Debug.Assert(_model != null);

            _element = element;
            Debug.Assert(_element != null);
        }

        public ActionType Type => TypeName;
        public string Title => "Move down element";
        public string Description => $"element={_element.Fullname}";

        public object Do()
        {
            IDsmElement nextElement = _model.NextSibling(_element);
            Debug.Assert(nextElement != null);

            _model.Swap(_element, nextElement);
            _model.AssignElementOrder();
            return null;
        }

        public void Undo()
        {
            IDsmElement previousElement = _model.PreviousSibling(_element);
            Debug.Assert(previousElement != null);

            _model.Swap(previousElement, _element);
            _model.AssignElementOrder();
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                return attributes.Data;
            }
        }
    }
}

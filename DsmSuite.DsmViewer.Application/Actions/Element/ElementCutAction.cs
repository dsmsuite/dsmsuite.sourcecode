using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementCutAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IActionContext _actionContext;
        private readonly IDsmElement _element;

        public const ActionType RegisteredType = ActionType.ElementCut;

        public ElementCutAction(object[] args)
        {
            if (args.Length == 3)
            {
                _model = args[0] as IDsmModel;
                _actionContext = args[1] as IActionContext;
                IReadOnlyDictionary<string, string> data = args[2] as IReadOnlyDictionary<string, string>;

                if ((_model != null) && (data != null))
                {
                    ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(_model, data);

                    _element = attributes.GetElement(nameof(_element));
                }
            }
        }

        public ElementCutAction(IDsmModel model, IDsmElement element, IActionContext actionContext)
        {
            _model = model;
            _element = element;
            _actionContext = actionContext;
        }

        public ActionType Type => RegisteredType;
        public string Title => "Cut element";
        public string Description => $"element={_element.Fullname}";

        public object Do()
        {
            _actionContext.AddElementToClipboard(_element);
            _model.RemoveElement(_element.Id);
            _model.AssignElementOrder();
            return null;
        }

        public void Undo()
        {
            _actionContext.RemoveElementFromClipboard(_element);
            _model.UnremoveElement(_element.Id);
            _model.AssignElementOrder();
        }

        public bool IsValid()
        {
            return (_model != null) &&
                   (_element != null);
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

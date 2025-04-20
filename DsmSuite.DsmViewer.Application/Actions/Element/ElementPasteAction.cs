using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPasteAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IActionContext _actionContext;
        private readonly IDsmElement _element;
        private readonly IDsmElement _oldParent;
        private readonly int _oldIndex;
        private readonly string _oldName;
        private readonly IDsmElement _newParent;
        private readonly int _newIndex;
        private string _newName;

        public const ActionType RegisteredType = ActionType.ElementPaste;

        public ElementPasteAction(IDsmModel model, IDsmElement newParent, int index, IActionContext actionContext)
        {
            _model = model;

            _actionContext = actionContext;
            _element = _actionContext.GetElementOnClipboard();

            _oldParent = _element.Parent;
            _oldIndex = _oldParent.IndexOfChild(_element);
            _oldName = _element.Name;

            _newParent = newParent;
            _newIndex = index;
            _newName = _element.Name;
        }

        public ActionType Type => RegisteredType;
        public string Title => "Paste element";
        public string Description => $"element={_actionContext.GetElementOnClipboard().Fullname}";

        public object Do()
        {
            // Rename to avoid duplicate names
            if (_newParent.ContainsChildWithName(_oldName))
            {
                _newName += " (duplicate)";
                _model.ChangeElementName(_element, _newName);
            }

            _model.ChangeElementParent(_element, _newParent, _newIndex);
            _model.AssignElementOrder();

            _actionContext.RemoveElementFromClipboard(_element);
            return null;
        }

        public void Undo()
        {
            _model.ChangeElementParent(_element, _oldParent, _oldIndex);
            _model.AssignElementOrder();

            // Restore original name
            if (_oldName != _newName)
            {
                _model.ChangeElementName(_element, _oldName);
            }
        }

        public bool IsValid()
        {
            return (_model != null) &&
                   (_element != null) &&
                   (_actionContext != null) &&
                   (_actionContext.IsElementOnClipboard()) &&
                   (_oldParent != null) &&
                   (_oldName != null) &&
                   (_newParent != null) &&
                   (_newName != null);
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

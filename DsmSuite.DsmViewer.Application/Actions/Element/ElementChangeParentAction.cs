using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementChangeParentAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly IDsmElement _old;
        private readonly int _oldIndex;
        private readonly string _oldName;
        private readonly IDsmElement _new;
        private readonly int _newIndex;
        private readonly string _newName;

        public const ActionType RegisteredType = ActionType.ElementChangeParent;

        public ElementChangeParentAction(object[] args)
        {
            if (args.Length == 2)
            {
                _model = args[0] as IDsmModel;
                IReadOnlyDictionary<string, string> data = args[1] as IReadOnlyDictionary<string, string>;

                if ((_model != null) && (data != null))
                {
                    ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(_model, data);

                    _element = attributes.GetElement(nameof(_element));
                    _old = attributes.GetElement(nameof(_old));
                    _oldIndex = attributes.GetInt(nameof(_oldIndex));
                    _oldName = attributes.GetString(nameof(_oldName));
                    _new = attributes.GetElement(nameof(_new));
                    _newIndex = attributes.GetInt(nameof(_newIndex));
                    _newName = attributes.GetString(nameof(_newName));
                }
            }
        }

        public ElementChangeParentAction(IDsmModel model, IDsmElement element, IDsmElement newParent, int index)
        {
            _model = model;
            _element = element;
            _old = element.Parent;
            _oldIndex = _old.IndexOfChild(element);
            _oldName = element.Name;
            _new = newParent;
            _newIndex = index;
            _newName = _oldName;
            if (_new.ContainsChildWithName(_oldName))
            {
                _newName += " (duplicate)";
            }
        }

        public ActionType Type => RegisteredType;
        public string Title => "Change element parent";
        public string Description => $"element={_element.Fullname} parent={_old.Fullname}->{_new.Fullname}";

        public object Do()
        {
            if (_oldName != _newName)
            {
                _model.ChangeElementName(_element, _newName);
            }
            _model.ChangeElementParent(_element, _new, _newIndex);
            _model.AssignElementOrder();
            return null;
        }

        public void Undo()
        {
            _model.ChangeElementParent(_element, _old, _oldIndex);
            _model.AssignElementOrder();

            if (_oldName != _newName)
            {
                _model.ChangeElementName(_element, _oldName);
            }
        }

        public bool IsValid()
        {
            return (_model != null) && 
                   (_element != null) && 
                   (_old != null) && 
                   (_oldName != null) && 
                   (_new != null) && 
                   (_newName != null);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetInt(nameof(_old), _old.Id);
                attributes.SetString(nameof(_oldName), _oldName);
                attributes.SetInt(nameof(_oldIndex), _oldIndex);
                attributes.SetInt(nameof(_new), _new.Id);
                attributes.SetString(nameof(_newName), _newName);
                attributes.SetInt(nameof(_newIndex), _newIndex);
                return attributes.Data;
            }
        }
    }
}

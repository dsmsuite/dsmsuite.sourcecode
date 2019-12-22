using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly IDsmElement _old;
        private readonly IDsmElement _new;

        public const string TypeName = "emove";

        public ElementMoveAction(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            IReadOnlyDictionary<string, string> data = args[1] as IReadOnlyDictionary<string, string>;
            Debug.Assert(data != null);

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = _model.GetElementById(id);
            Debug.Assert(_element != null);

            int oldParentid = attributes.GetInt(nameof(_old));
            _old = _model.GetElementById(oldParentid);
            Debug.Assert(_old != null);

            int newParentId = attributes.GetInt(nameof(_new));
            _new = _model.GetElementById(newParentId);
            Debug.Assert(_new != null);
        }

        public ElementMoveAction(IDsmModel model, IDsmElement element, IDsmElement newParent)
        {
            _model = model;
            Debug.Assert(_model != null);

            _element = element;
            Debug.Assert(_element != null);

            _old = element.Parent;
            Debug.Assert(_old != null);

            _new = newParent;
            Debug.Assert(_new != null);
        }

        public string Type => TypeName;
        public string Title => "Move element";
        public string Description => $"element={_element.Fullname} parent={_old.Fullname}->{_new.Fullname}";

        public void Do()
        {
            _model.ChangeParent(_element, _new);
        }

        public void Undo()
        {
            _model.ChangeParent(_element, _old);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetInt(nameof(_old), _old.Id);
                attributes.SetInt(nameof(_new), _new.Id);
                return attributes.Data;
            }
        }
    }
}

using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementEditTypeAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly string _old;
        private readonly string _new;

        public const string TypeName = "eedittype";

        public ElementEditTypeAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            _model = model;

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _old = attributes.GetString(nameof(_old));
            _new = attributes.GetString(nameof(_new));
       }

        public ElementEditTypeAction(IDsmModel model, IDsmElement element, string type)
        {
            _model = model;
            _element = element;
            Debug.Assert(_element != null);

            _old = _element.Type;
            _new = type;
        }

        public string Type => TypeName;
        public string Title => "Edit element type";
        public string Description => $"element={_element.Fullname} type={_old}->{_new}";

        public void Do()
        {
            _model.EditElementType(_element, _new);
        }

        public void Undo()
        {
            _model.EditElementType(_element, _old);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetString(nameof(_old), _old);
                attributes.SetString(nameof(_new), _new);
                return attributes.Data;
            }
        }
    }
}

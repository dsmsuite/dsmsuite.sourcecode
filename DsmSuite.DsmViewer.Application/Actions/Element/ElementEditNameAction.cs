using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementEditNameAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly string _old;
        private readonly string _new;

        public const string TypeName = "eeditname";

        public ElementEditNameAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            _model = model;

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _old = attributes.GetString(nameof(_old));
            _new = attributes.GetString(nameof(_new));
        }

        public ElementEditNameAction(IDsmModel model, IDsmElement element, string name)
        {
            _model = model;
            _element = element;
            Debug.Assert(_element != null);

            _old = _element.Name;
            _new = name;
        }

        public string Type => TypeName;
        public string Title => "Edit element name";
        public string Description => $"element={_element.Fullname} name={_old}->{_new}";

        public void Do()
        {
            _model.EditElementName(_element, _new);
        }

        public void Undo()
        {
            _model.EditElementName(_element, _old);
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

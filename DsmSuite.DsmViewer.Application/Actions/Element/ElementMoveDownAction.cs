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

        public const string TypeName = "emovedown";
    
        public ElementMoveDownAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            _model = model;

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);
        }

        public ElementMoveDownAction(IDsmModel model, IDsmElement element)
        {
            _model = model;
            _element = element;
            Debug.Assert(_element != null);
        }

        public string Type => TypeName;
        public string Title => "Move down element";
        public string Description => $"element={_element.Fullname}";

        public void Do()
        {
            IDsmElement nextElement = _model.NextSibling(_element);
            Debug.Assert(nextElement != null);

            _model.Swap(_element, nextElement);
        }

        public void Undo()
        {
            IDsmElement previousElement = _model.PreviousSibling(_element);
            Debug.Assert(previousElement != null);

            _model.Swap(previousElement, _element);
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

using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementMoveUpAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;

        public const string TypeName = "emoveup";

        public ElementMoveUpAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            _model = model;

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);
        }

        public ElementMoveUpAction(IDsmModel model, IDsmElement element)
        {
            _model = model;

            _element = element;
            Debug.Assert(_element != null);
        }

        public string Type => TypeName;
        public string Title => "Move up element";
        public string Description => $"element={_element.Fullname}";

        public void Do()
        {
            IDsmElement previousElement = _model.PreviousSibling(_element);
            Debug.Assert(previousElement != null);

            _model.Swap(_element, previousElement);
        }

        public void Undo()
        {
            IDsmElement nextElement = _model.NextSibling(_element);
            Debug.Assert(nextElement != null);

            _model.Swap(_element, nextElement);
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

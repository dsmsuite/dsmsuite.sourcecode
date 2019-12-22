using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementDeleteAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;

        public const string TypeName = "edelete";

        public ElementDeleteAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            _model = model;

            ReadOnlyActionAttributes attributes = new ReadOnlyActionAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetDeletedElementById(id);
            Debug.Assert(_element != null);
        }

        public ElementDeleteAction(IDsmModel model, IDsmElement element)
        {
            _model = model;
            _element = element;
            Debug.Assert(_element != null);
        }

        public string Type => TypeName;
        public string Title => "Delete element";
        public string Description => $"element={_element.Fullname}";

        public void Do()
        {
            _model.RemoveElement(_element.Id);
        }

        public void Undo()
        {
            _model.UnremoveElement(_element.Id);
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

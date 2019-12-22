using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPartitionAction : IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly string _algorithm;
        private IElementSequence _vector;

        public const string TypeName = "epartition";

        public ElementPartitionAction(IDsmModel model, IReadOnlyDictionary<string, string> data)
        {
            _model = model;

            ActionReadOnlyAttributes attributes = new ActionReadOnlyAttributes(data);
            int id = attributes.GetInt(nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _algorithm = attributes.GetString(nameof(Algorithm));
        }

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm)
        {
            _model = model;
            _element = element;
            Debug.Assert(_element != null);

            _algorithm = algorithm;
        }

        public string Type => TypeName;
        public string Title => "Partition element";
        public string Description => $"element={_element.Fullname} algorithm={_algorithm}";

        public void Do()
        {
            Partitioner partitioner = new Partitioner(_element, _model);
            _vector = partitioner.Partition();
            _model.ReorderChildren(_element, _vector);
        }

        public void Undo()
        {
            ElementSequence inverseVector = new ElementSequence(_vector.GetNumberOfElements());
            for (int i = 0; i < _vector.GetNumberOfElements(); i++)
            {
                inverseVector.SetIndex(_vector.GetIndex(i), i);
            }
            _model.ReorderChildren(_element, inverseVector);
        }

        public IReadOnlyDictionary<string, string> Data
        {
            get
            {
                ActionAttributes attributes = new ActionAttributes();
                attributes.SetInt(nameof(_element), _element.Id);
                attributes.SetString(nameof(_algorithm), _algorithm);
                return attributes.Data;
            }
        }
    }
}

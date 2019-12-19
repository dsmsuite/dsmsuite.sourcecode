using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPartitionAction : ActionBase
    {
        private readonly IDsmElement _element;
        private readonly string _algorithm;
        private IElementSequence _vector;

        public ElementPartitionAction(IDsmModel model, IReadOnlyDictionary<string, string> data) : base(model)
        {
            int id = GetInt(data, nameof(_element));
            _element = model.GetElementById(id);
            Debug.Assert(_element != null);

            _algorithm = GetString(data, nameof(Algorithm));
        }

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm) : base(model)
        {
            _element = element;
            Debug.Assert(_element != null);

            _algorithm = algorithm;
        }

        public override string ActionName => nameof(ElementPartitionAction);
        public override string Title => "Partition element";
        public override string Description => $"element={_element.Fullname} algorithm={_algorithm}";

        public override void Do()
        {
            Partitioner partitioner = new Partitioner(_element, Model);
            _vector = partitioner.Partition();
            Model.ReorderChildren(_element, _vector);
        }

        public override void Undo()
        {
            ElementSequence inverseVector = new ElementSequence(_vector.GetNumberOfElements());
            for (int i = 0; i < _vector.GetNumberOfElements(); i++)
            {
                inverseVector.SetIndex(_vector.GetIndex(i), i);
            }
            Model.ReorderChildren(_element, inverseVector);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            SetInt(data, nameof(_element), _element.Id);
            SetString(data, nameof(_algorithm), _algorithm);
            return data;
        }
    }
}

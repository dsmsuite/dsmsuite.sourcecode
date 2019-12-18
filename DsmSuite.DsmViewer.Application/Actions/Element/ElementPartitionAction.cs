using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPartitionAction : ActionBase
    {
        private readonly int _elementId;
        private readonly string _algorithm;
        private IElementSequence _vector;

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm) : base(model)
        {
            _elementId = element.Id;
            _algorithm = algorithm;

            Type = "Partition element";
            Details = $"name={element.Fullname} algorithm={algorithm}";
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element != null)
            {
                Partitioner partitioner = new Partitioner(element, Model);
                _vector = partitioner.Partition();
                Model.ReorderChildren(element, _vector);
            }
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(_elementId);
            if (element != null)
            {
                ElementSequence inverseVector = new ElementSequence(_vector.GetNumberOfElements());
                for (int i = 0; i < _vector.GetNumberOfElements(); i++)
                {
                    inverseVector.SetIndex(_vector.GetIndex(i), i);
                }
                Model.ReorderChildren(element, inverseVector);
            }
        }
    }
}

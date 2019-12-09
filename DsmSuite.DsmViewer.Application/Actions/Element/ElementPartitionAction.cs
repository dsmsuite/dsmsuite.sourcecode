using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPartitionAction : ActionBase
    {
        private readonly IDsmElement _element;
        private readonly  string _algorithm;
        private Vector _vector;

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm) : base(model)
        {
            _element = element;
            _algorithm = algorithm;
        }

        public override void Do()
        {
            Partitioner partitioner = new Partitioner(_element, Model);
            _vector = partitioner.Partition();
            Model.ReorderChildren(_element, _vector);
        }

        public override void Undo()
        {
            Vector inverseVector = new Vector(_vector.Size());
            for (int i = 0; i < _vector.Size(); i++)
            {
                inverseVector.Set(_vector.Get(i), i);
            }
            Model.ReorderChildren(_element, inverseVector);
        }

        public override string Type => "Partition element";
        public override string Details => "name={_element.Fullname} algorithm={_algorithm}";
    }
}

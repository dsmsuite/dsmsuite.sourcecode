using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPartitionAction : ActionBase
    {
        public int Element { get; }
        public string Algorithm { get; }
        private IElementSequence _vector; 

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm) : base(model)
        {
            Element = element.Id;
            Algorithm = algorithm;

            ClassName = nameof(ElementPartitionAction);
            Title = "Partition element";
            Details = $"name={element.Fullname} algorithm={algorithm}";
        }

        public override void Do()
        {
            IDsmElement element = Model.GetElementById(Element);
            Debug.Assert(element != null);

            Partitioner partitioner = new Partitioner(element, Model);
            _vector = partitioner.Partition();
            Model.ReorderChildren(element, _vector);
        }

        public override void Undo()
        {
            IDsmElement element = Model.GetElementById(Element);
            Debug.Assert(element != null);

            ElementSequence inverseVector = new ElementSequence(_vector.GetNumberOfElements());
            for (int i = 0; i < _vector.GetNumberOfElements(); i++)
            {
                inverseVector.SetIndex(_vector.GetIndex(i), i);
            }
            Model.ReorderChildren(element, inverseVector);
        }

        public override IReadOnlyDictionary<string, string> Pack()
        {
            return null;
        }

        public override IAction Unpack(IReadOnlyDictionary<string, string> data)
        {
            return null;
        }
    }
}

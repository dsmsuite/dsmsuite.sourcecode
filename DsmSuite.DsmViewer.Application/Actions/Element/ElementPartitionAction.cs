using System;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Actions.Element
{
    public class ElementPartitionAction : ActionBase, IAction
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;
        private readonly  string _algorithm;
        private Vector _vector;

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm) : base(model)
        {
            _model = model;
            _element = element;
            _algorithm = algorithm;
        }

        public void Do()
        {
            Partitioner partitioner = new Partitioner(_element, _model);
            _vector = partitioner.Partition();
            _model.ReorderChildren(_element, _vector);
        }

        public void Undo()
        {
            Vector inverseVector = new Vector(_vector.Size());
            for (int i = 0; i < _vector.Size(); i++)
            {
                inverseVector.Set(_vector.Get(i), i);
            }
            _model.ReorderChildren(_element, inverseVector);
        }

        public string Description => "Partition element";
    }
}

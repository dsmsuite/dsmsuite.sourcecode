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

        public ElementPartitionAction(IDsmModel model, IDsmElement element, string algorithm) : base(model)
        {
            _model = model;
            _element = element;
            _algorithm = algorithm;
        }

        public void Do()
        {
            Partitioner partitioner = new Partitioner(_element, _model);
            Vector vector = partitioner.Partition();
            _model.ReorderChildren(_element, vector);
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }

        public string Description => "Partition element";
    }
}

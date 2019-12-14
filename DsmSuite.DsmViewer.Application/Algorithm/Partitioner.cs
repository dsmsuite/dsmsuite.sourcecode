using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    internal class Partitioner
    {
        private readonly DsmElement _element;
        private readonly IDsmModel _model;

        public Partitioner(IDsmElement element, IDsmModel model)
        {
            _element = element as DsmElement;
            _model = model;
        }

        public IElementSequence Partition()
        {
            IElementSequence vector = new ElementSequence(_element.Children.Count);
            if (_element.Children.Count > 1)
            {
                SquareMatrix matrix = BuildPartitionMatrix(_element.Children);

                PartitioningAlgorithm algorithm = new PartitioningAlgorithm(matrix);

                vector = algorithm.Partition();
            }

            return vector;
        }

        SquareMatrix BuildPartitionMatrix(IList<IDsmElement> nodes)
        {
            SquareMatrix matrix = new SquareMatrix(nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                IDsmElement provider = nodes[i];

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (j != i)
                    {
                        IDsmElement consumer = nodes[j];

                        int weight = _model.GetDependencyWeight(consumer.Id, provider.Id);

                        matrix.Set(i, j, weight > 0 ? 1 : 0);
                    }
                }
            }

            return matrix;
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.DsmViewer.Model.Dependencies;

namespace DsmSuite.DsmViewer.Model.Algorithms
{
    internal class Partitioner
    {
        private readonly Element _element;
        private readonly IDsmModel _model;

        public Partitioner(IElement element, IDsmModel model)
        {
            _element = element as Element;
            Debug.Assert(_element != null);

            _model = model;
            Debug.Assert(_model != null);
        }

        public void Partition()
        {
            PartitionGroup(_element.Children);
        }

        void PartitionGroup(IList<IElement> nodes)
        {
            if (nodes.Count > 1)
            {
                SquareMatrix matrix = BuildPartitionMatrix(nodes);

                PartitioningAlgorithm p = new PartitioningAlgorithm(matrix);

                Vector v = p.Partition();

                ReorderNodes(nodes, v);
            }
        }

        SquareMatrix BuildPartitionMatrix(IList<IElement> nodes)
        {
            SquareMatrix matrix = new SquareMatrix(nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                IElement provider = nodes[i];

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (j != i)
                    {
                        IElement consumer = nodes[j];

                        int weight = _model.GetDependencyWeight(consumer, provider);

                        matrix.Set(i, j, weight > 0 ? 1 : 0);
                    }
                }
            }

            return matrix;
        }

        void ReorderNodes(IList<IElement> nodes, Vector permutationVector)
        {
            List<IElement> elements = new List<IElement>(nodes); // Clone before modify list

            foreach (IElement node in elements)
            {
                Debug.Assert(node.Parent == _element);
                _element.RemoveChild(node);
            }

            for (int i = 0; i < permutationVector.Size; i++)
            {
                _element.AddChild(elements[permutationVector.Get(i)]);
            }
        }
    }
}

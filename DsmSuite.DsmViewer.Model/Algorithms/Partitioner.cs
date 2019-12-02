using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Algorithms
{
    internal class Partitioner
    {
        private readonly DsmElement _element;
        private readonly IDsmModel _model;

        public Partitioner(IDsmElement element, IDsmModel model)
        {
            _element = element as DsmElement;
            Debug.Assert(_element != null);

            _model = model;
            Debug.Assert(_model != null);
        }

        public void Partition()
        {
            PartitionGroup(_element.Children);
        }

        void PartitionGroup(IList<IDsmElement> nodes)
        {
            if (nodes.Count > 1)
            {
                SquareMatrix matrix = BuildPartitionMatrix(nodes);

                PartitioningAlgorithm p = new PartitioningAlgorithm(matrix);

                Vector v = p.Partition();

                ReorderNodes(nodes, v);
            }
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

                        int weight = _model.GetDependencyWeight(consumer, provider);

                        matrix.Set(i, j, weight > 0 ? 1 : 0);
                    }
                }
            }

            return matrix;
        }

        void ReorderNodes(IList<IDsmElement> nodes, Vector permutationVector)
        {
            List<IDsmElement> elements = new List<IDsmElement>(nodes); // Clone before modify list

            foreach (IDsmElement node in elements)
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

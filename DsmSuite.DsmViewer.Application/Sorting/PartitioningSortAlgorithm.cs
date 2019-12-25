using System.Collections.Generic;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Diagnostics;

namespace DsmSuite.DsmViewer.Application.Algorithm
{
    internal class PartitionSortAlgorithm : ISortAlgorithm
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;

        public const string AlgorithmName = "Partition";

        public PartitionSortAlgorithm(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            _element = args[1] as IDsmElement;
            Debug.Assert(_element != null);
        }

        public SortResult Sort()
        {
            SortResult vector = new SortResult(_element.Children.Count);
            if (_element.Children.Count > 1)
            {
                WeightsMatrix matrix = BuildPartitionMatrix(_element.Children);

                PartitioningCalculation algorithm = new PartitioningCalculation(matrix);

                vector = algorithm.Partition();
            }

            return vector;
        }

        public string Name => AlgorithmName;

        private WeightsMatrix BuildPartitionMatrix(IList<IDsmElement> nodes)
        {
            WeightsMatrix matrix = new WeightsMatrix(nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                IDsmElement provider = nodes[i];

                for (int j = 0; j < nodes.Count; j++)
                {
                    if (j != i)
                    {
                        IDsmElement consumer = nodes[j];

                        int weight = _model.GetDependencyWeight(consumer.Id, provider.Id);

                        matrix.SetWeight(i, j, weight > 0 ? 1 : 0);
                    }
                }
            }

            return matrix;
        }
    }
}

using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DsmSuite.DsmViewer.Application.Sorting
{
    public class AlphabeticalSortAlgorithm : ISortAlgorithm
    {
        private readonly IDsmModel _model;
        private readonly IDsmElement _element;

        public const string AlgorithmName = "Alphabetical";

        public AlphabeticalSortAlgorithm(object[] args)
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
                List<int> newOrder = _element.Children.OrderBy(x => x.Name).Select(x => x.Id).ToList();

                for (int i=0; i< vector.GetNumberOfElements(); i++)
                {
                    int id = _element.Children[i].Id;
                    vector.SetIndex(newOrder.IndexOf(id), i);
                }
            }

            return vector;
        }

        public string Name => AlgorithmName;
    }
}

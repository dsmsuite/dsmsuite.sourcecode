using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.DsmViewer.Application.Sorting
{
    internal class AlphabeticalSortAlgorithm
    {
        private readonly IDsmModel _model;
        private readonly DsmElement _element;

        public const string AlgorithmName = "Alphabetical";

        public AlphabeticalSortAlgorithm(object[] args)
        {
            Debug.Assert(args.Length == 2);
            _model = args[0] as IDsmModel;
            Debug.Assert(_model != null);
            _element = args[1] as DsmElement;
            Debug.Assert(_element != null);
        }

        public SortResult Sort()
        {
            SortResult vector = new SortResult(_element.Children.Count);
            if (_element.Children.Count > 1)
            {
                List<IDsmElement> sortedChilderen = _element.Children.OrderBy(x => x.Name).ToList();
            }

            return vector;
        }

        public string Name => AlgorithmName;
    }
}

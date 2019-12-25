using DsmSuite.DsmViewer.Application.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.DsmViewer.Application.Test.Stubs
{
    public class StubbedSortAlgorithm : ISortAlgorithm
    {
        private const string _sortResult = "2,0,1";
        private const string _inverseSortResult = "1,2,0";

        public StubbedSortAlgorithm(object[] args) { }

        public string Name => "Stub";

        public SortResult Sort()
        {
            return new SortResult(_sortResult);
        }
    }
}

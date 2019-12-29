using System;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Metrics
{
    public class DsmMetrics
    {
        private readonly IDsmModel _model;

        public DsmMetrics(IDsmModel model)
        {
            _model = model;
        }

        public int GetElementSize(IDsmElement elenent)
        {
            int count = 0;
            CountChildern(elenent, ref count);
            return count;
        }

        private void CountChildern(IDsmElement element, ref int count)
        {
            count++;

            foreach (IDsmElement child in element.Children)
            {
                CountChildern(child, ref count);
            }
        }
    }
}

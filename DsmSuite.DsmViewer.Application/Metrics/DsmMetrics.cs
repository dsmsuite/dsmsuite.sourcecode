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
    }
}

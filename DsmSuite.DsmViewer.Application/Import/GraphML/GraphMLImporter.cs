using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Sorting;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Import.Common;

namespace DsmSuite.DsmViewer.Application.Import.GraphML
{
    public class GraphMLImporter : ImporterBase
    {
        private readonly IDsiModel _dsiModel;
        private readonly IDsmModel _dsmModel;
        private readonly IImportPolicy _importPolicy;
        private readonly bool _autoPartition;

        public GraphMLImporter(IDsiModel dsiModel, IDsmModel dsmModel, IImportPolicy importPolicy, bool autoPartition) : base(dsmModel)
        {
            _dsiModel = dsiModel;
            _dsmModel = dsmModel;
            _importPolicy = importPolicy;
            _autoPartition = autoPartition;
        }

        public void Import(IProgress<ProgressInfo> progress)
        {
            ImportElements(progress);
            ImportRelations(progress);

            if (_autoPartition)
            {
                Partition(progress);
            }
        }

        private void ImportElements(IProgress<ProgressInfo> progress)
        {
        }

        private void ImportRelations(IProgress<ProgressInfo> progress)
        {
        }
    }
}

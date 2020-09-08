using System;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Application.Import.Common;

namespace DsmSuite.DsmViewer.Application.Import.GraphML
{
    public class GraphMlImporter : ImporterBase
    {
        private readonly string _graphMlFilename;
        private readonly IDsmModel _dsmModel;
        private readonly IImportPolicy _importPolicy;
        private readonly bool _autoPartition;

        public GraphMlImporter(string graphMlFilename, IDsmModel dsmModel, IImportPolicy importPolicy, bool autoPartition) : base(dsmModel)
        {
            _graphMlFilename = graphMlFilename;
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

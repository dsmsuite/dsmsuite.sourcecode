using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Import.Common;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import.Dsi
{
    public class SqlImporter
    {
        private readonly string _filename;
        private readonly IDsmBuilder _importPolicy;
        private readonly bool _autoPartition;
        private readonly Dictionary<int, int> _dsiToDsmMapping;

        public SqlImporter(string filename, IDsmModel dsmModel, IDsmBuilder importPolicy, bool autoPartition) 
        {
            _filename = filename;
            _importPolicy = importPolicy;
            _autoPartition = autoPartition;
            _dsiToDsmMapping = new Dictionary<int, int>();
        }

        public void Import(IProgress<ProgressInfo> progress)
        {
            _importPolicy.FinalizeImport(progress);
        }
    }
}

using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzers.Python.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzers.Python.Analysis
{
    public class Analyzer
    {
        private readonly IDsiModel _model;
        private readonly AnalyzerSettings _analyzerSettings;
        private readonly IProgress<ProgressInfo> _progress;

        public Analyzer(IDsiModel model, AnalyzerSettings analyzerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _analyzerSettings = analyzerSettings;
            _progress = progress;
        }

        public void Analyze()
        {
            int lineNumber = 0;

                FileInfo fileInfo = new FileInfo(_analyzerSettings.Input.JsonFilename);
                if (fileInfo.Extension == ".json")
                {
                }
  
            UpdateLineProgress(lineNumber, true);
        }

        private void RegisterRelation(string consumerName, string providerName)
        {
            _model.AddElement(consumerName, "", null);
            _model.AddElement(providerName, "", null);
            _model.AddRelation(consumerName, providerName, "dependency", 1, null);
        }

        private void UpdateLineProgress(int lineNumber, bool done)
        {
            ProgressInfo progressInfo = new ProgressInfo
            {
                ActionText = "Parse lines",
                CurrentItemCount = lineNumber,
                TotalItemCount = 0,
                ItemType = "lines",
                Percentage = null,
                Done = done
            };
            _progress?.Report(progressInfo);
        }
    }
}

using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.Analyzer.Data;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Transformer.Transformation
{
    public class Transformer
    {
        private readonly IDataModel _model;
        private readonly TransformerSettings _transformerSettings;

        public Transformer(IDataModel model, TransformerSettings transformerSettings)
        {
            _model = model;
            _transformerSettings = transformerSettings;
        }

        public void Transform()
        {
            List<Action> actions = new List<Action>();

            actions.Add(new IncludeFilterAction(_model, _transformerSettings.IncludeFilterSettings));
            actions.Add(new PreFixSingleRootAction(_model, _transformerSettings.PreFixSingleRootSettings.Enabled));
            actions.Add(new MoveHeaderElementsAction(_model, _transformerSettings.MoveHeaderElementsSettings.Enabled));
            actions.Add(new MoveElementsAction(_model, _transformerSettings.MoveElementsSettings));
            actions.Add(new AddTransitiveRelationsAction(_model, _transformerSettings.AddTransitiveRelationsSettings.Enabled));
            actions.Add(new SplitProductAndTestElementsAction(_model, _transformerSettings.SplitProductAndTestElementsSettings));
            
            foreach (Action action in actions)
            {
                _model.AddMetaData(action.Name, action.IsEnabled ? "Enabled" : "Disabled");
                Logger.LogUserMessage($"Transforming: {action.Name}");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                action.Execute();

                stopWatch.Stop();
                Logger.LogUserMessage($" total elapsed time={stopWatch.Elapsed}");
            }

            Process currentProcess = Process.GetCurrentProcess();
            const long million = 1000000;
            long peakPagedMemMb = currentProcess.PeakPagedMemorySize64 / million;
            long peakVirtualMemMb = currentProcess.PeakVirtualMemorySize64 / million;
            long peakWorkingSetMb = currentProcess.PeakWorkingSet64 / million;
            Logger.LogUserMessage($" peak physical memory usage {peakWorkingSetMb:0.000}MB");
            Logger.LogUserMessage($" peak paged memory usage    {peakPagedMemMb:0.000}MB");
            Logger.LogUserMessage($" peak virtual memory usage  {peakVirtualMemMb:0.000}MB");
        }
    }
}

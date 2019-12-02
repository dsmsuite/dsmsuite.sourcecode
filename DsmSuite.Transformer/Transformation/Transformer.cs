using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    public class Transformer
    {
        private readonly IDsiDataModel _model;
        private readonly TransformerSettings _transformerSettings;

        public Transformer(IDsiDataModel model, TransformerSettings transformerSettings)
        {
            _model = model;
            _transformerSettings = transformerSettings;
        }

        public void Transform()
        {
            List<Action> actions = new List<Action>
            {
                new IncludeFilterAction(_model, _transformerSettings.IncludeFilterSettings),
                new PreFixSingleRootAction(_model, _transformerSettings.PreFixSingleRootSettings.Enabled),
                new MoveHeaderElementsAction(_model, _transformerSettings.MoveHeaderElementsSettings.Enabled),
                new MoveElementsAction(_model, _transformerSettings.MoveElementsSettings),
                new AddTransitiveRelationsAction(_model, _transformerSettings.AddTransitiveRelationsSettings.Enabled),
                new SplitProductAndTestElementsAction(_model, _transformerSettings.SplitProductAndTestElementsSettings)
            };


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

            Logger.LogResourceUsage();
        }
    }
}

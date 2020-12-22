using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    public class Transformer
    {
        private readonly IDsiModel _model;
        private readonly TransformerSettings _transformerSettings;
        private readonly IProgress<ProgressInfo> _progress;

        public Transformer(IDsiModel model, TransformerSettings transformerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _transformerSettings = transformerSettings;
            _progress = progress;
        }

        public void Transform()
        {
            List<Action> actions = new List<Action>
            {
                new IncludeFilterAction(_model, _transformerSettings.IncludeFilterSettings, _progress),
                new MoveHeaderElementsAction(_model, _transformerSettings.MoveHeaderElementsSettings.Enabled, _progress),
                new MoveElementsAction(_model, _transformerSettings.MoveElementsSettings, _progress),
                new AddTransitiveRelationsAction(_model, _transformerSettings.AddTransitiveRelationsSettings.Enabled, _progress),
                new SplitProductAndTestElementsAction(_model, _transformerSettings.SplitProductAndTestElementsSettings, _progress)
            };

            foreach (Action action in actions)
            {
                _model.AddMetaData(action.Name, action.IsEnabled ? "Enabled" : "Disabled");
                action.Execute();
            }
        }
    }
}

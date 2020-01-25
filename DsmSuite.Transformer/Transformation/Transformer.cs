using System;
using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    public class Transformer
    {
        private readonly IDsiModel _model;
        private readonly TransformerSettings _transformerSettings;

        public Transformer(IDsiModel model, TransformerSettings transformerSettings)
        {
            _model = model;
            _transformerSettings = transformerSettings;
        }

        public void Transform(IProgress<ProgressInfo> progress)
        {
            List<Action> actions = new List<Action>
            {
                new IncludeFilterAction(_model, _transformerSettings.IncludeFilterSettings),
                new MoveHeaderElementsAction(_model, _transformerSettings.MoveHeaderElementsSettings.Enabled),
                new MoveElementsAction(_model, _transformerSettings.MoveElementsSettings),
                new AddTransitiveRelationsAction(_model, _transformerSettings.AddTransitiveRelationsSettings.Enabled),
                new SplitProductAndTestElementsAction(_model, _transformerSettings.SplitProductAndTestElementsSettings)
            };

            foreach (Action action in actions)
            {
                _model.AddMetaData(action.Name, action.IsEnabled ? "Enabled" : "Disabled");
                action.Execute(progress);
            }
        }
    }
}

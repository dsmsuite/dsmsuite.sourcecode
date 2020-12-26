using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Transformation;
using DsmSuite.Analyzer.VisualStudio.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.VisualStudio.Transformation
{
    public class Transformer
    {
        private readonly IDsiModel _model;
        private readonly TransformationSettings _transformerSettings;
        private readonly IProgress<ProgressInfo> _progress;

        public Transformer(IDsiModel model, TransformationSettings transformerSettings, IProgress<ProgressInfo> progress)
        {
            _model = model;
            _transformerSettings = transformerSettings;
            _progress = progress;
        }

        public void Transform()
        {
            List<TransformationAction> actions = new List<TransformationAction>();

            if (_transformerSettings.AddTransitiveIncludes)
            {
                actions.Add(new AddTransitiveRelationsTransformationAction(_model,  _progress));
            }

            if (_transformerSettings.IgnoredNames.Count > 0)
            {
                actions.Add(new RemoveIgnoreNamesTransformationAction(_model, _transformerSettings.IgnoredNames, _progress));
            }

            switch (_transformerSettings.HeaderSourceFileMergeStrategy)
            {
                case TransformationHeaderSourceFileMergeStrategy.None:
                {
                }
                    break;
                case TransformationHeaderSourceFileMergeStrategy.MoveHeaderFileToSourceFile:
                {
                    actions.Add(new MoveHeaderToSourceFileDirectoryTransformationAction(_model, _progress));
                    }
                    break;
                case TransformationHeaderSourceFileMergeStrategy.MergeHeaderAndSourceFileDirectory:
                {
                    actions.Add(new MergeHeaderAndSourceFileDirectoriesTransformationAction(_model, _transformerSettings.MergeHeaderAndSourceFileDirectoryRules, _progress));
                    }
                    break;
            }

            foreach (TransformationAction action in actions)
            {
                action.Execute();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Cpp.Settings;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Transformation;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Cpp.Transformation
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
                actions.Add(new AddTransitiveRelationsTransformationAction(_model, _progress));
            }

            switch (_transformerSettings.ModuleMergeStrategy)
            {
                case TransformationModuleMergeStrategy.None:
                    {
                    }
                    break;
                case TransformationModuleMergeStrategy.MoveHeaderFileToSourceFile:
                    {
                        actions.Add(new MoveHeaderToSourceFileDirectoryTransformationAction(_model, _progress));
                    }
                    break;
                case TransformationModuleMergeStrategy.MergeHeaderAndSourceFileDirectory:
                    {
                        actions.Add(new MergeHeaderAndSourceFileDirectoriesTransformationAction(_model, _transformerSettings.ModuleMergeRules, _progress));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DsmSuite.Analyzer.DotNet.Settings;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Transformation;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.DotNet.Transfromation
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

            if (_transformerSettings.IgnoredNames.Count > 0)
            {
                actions.Add(new RemoveIgnoreNamesTransformationAction(_model, _transformerSettings.IgnoredNames, _progress));
            }

            foreach (TransformationAction action in actions)
            {
                action.Execute();
            }
        }
    }
}

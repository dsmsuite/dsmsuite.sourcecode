using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Transformations.Settings;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Transformations.Transformation
{
    public class MergeHeaderAndSourceFileDirectoriesTransformationAction : TransformationAction
    {
        private const string ActionName = "Move elements according specified rules";
        private readonly IDsiModel _model;
        private readonly List<TransformationModuleMergeRule> _transformationRules;

        public MergeHeaderAndSourceFileDirectoriesTransformationAction(IDsiModel model, List<TransformationModuleMergeRule> transformationRules, IProgress<ProgressInfo> progress) :
            base(ActionName, progress)
        {
            _model = model;
            _transformationRules = transformationRules;
        }

        public override void Execute()
        {
            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration

            int totalElements = _model.GetElementCount();
            int transformedElements = 0;
            foreach (IDsiElement element in clonedElements)
            {
                MoveElement(element);

                transformedElements++;
                UpdateTransformationProgress(Name, transformedElements, totalElements);
            }
        }

        private void MoveElement(IDsiElement element)
        {
            foreach (TransformationModuleMergeRule rule in _transformationRules)
            {
                if (element.Name.Contains(rule.From))
                {
                    string elementName = element.Name;
                    string newElementName = element.Name.ReplaceIgnoreCase(rule.From, rule.To);

                    _model.RenameElement(element, newElementName);
                }
            }
        }
    }
}

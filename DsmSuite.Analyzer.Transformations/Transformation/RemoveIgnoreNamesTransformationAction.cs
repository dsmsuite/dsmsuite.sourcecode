using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Transformations.Transformation
{
    public class RemoveIgnoreNamesTransformationAction : TransformationAction
    {
        private const string ActionName = "Include filter";
        private readonly IDsiModel _model;
        private readonly List<string> _ignoredNames;

        public RemoveIgnoreNamesTransformationAction(IDsiModel model, List<string> ignoredNames, IProgress<ProgressInfo> progress) :
            base(ActionName, progress)
        {
            _model = model;
            _ignoredNames = ignoredNames;
        }

        public override void Execute()
        {
            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration

            int totalElements = _model.CurrentElementCount;
            int transformedElements = 0;
            foreach (IDsiElement element in clonedElements)
            {
                IncludeElement(element);

                transformedElements++;
                UpdateTransformationProgress(Name, transformedElements, totalElements);
            }
        }

        private void IncludeElement(IDsiElement element)
        {
            if (!ShouldElementBeIncluded(element))
            {
                _model.RemoveElement(element);
            }
        }

        private bool ShouldElementBeIncluded(IDsiElement element)
        {
            bool include = true;

            foreach (string name in _ignoredNames)
            {
                Regex regex = new Regex(name);
                Match match = regex.Match(element.Name);
                if (match.Success)
                {
                    include = false;
                }
            }
            return include;
        }
    }
}

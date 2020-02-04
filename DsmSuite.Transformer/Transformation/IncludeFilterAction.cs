using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    public class IncludeFilterAction : Action
    {
        private const string ActionName = "Include filter";
        private readonly IDsiModel _model;
        private readonly List<string> _names;

        public IncludeFilterAction(IDsiModel model, IncludeFilterSettings includeFilterSettings, IProgress<ProgressInfo> progress) :
            base(ActionName, includeFilterSettings.Enabled, progress)
        {
            _model = model;
            _names = includeFilterSettings.Names;
        }

        protected override void ExecuteImpl()
        {

            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration

            int totalElements = _model.GetElementCount();
            int transformedElements = 0;
            foreach (IDsiElement element in clonedElements)
            {
                IncludeElement(element);

                transformedElements++;
                UpdateTransformationProgress(transformedElements, totalElements);
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
            bool include = false;

            foreach (string name in _names)
            {
                Regex regex = new Regex(name);
                Match match = regex.Match(element.Name);
                if (match.Success)
                {
                    include = true;
                }
            }
            return include;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DsmSuite.Analyzer.Data;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    public class IncludeFilterAction : Action
    {
        private const string ActionName = "Include filter";
        private readonly IDataModel _model;
        private readonly List<string> _names;

        public IncludeFilterAction(IDataModel model, IncludeFilterSettings includeFilterSettings) :
            base(ActionName, includeFilterSettings.Enabled)
        {
            _model = model;
            _names = includeFilterSettings.Names;
        }

        protected override void ExecuteImpl()
        {
            int transformedElements = 0;
            IElement[] clonedElements = _model.Elements.ToArray(); // Because elements in collection change during iteration
            foreach (IElement element in clonedElements)
            {
                IncludeElement(element);

                transformedElements++;
                Console.Write("\r progress elements={0}", transformedElements);
            }

            _model.Cleanup();

            Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void IncludeElement(IElement element)
        {
            if (!ShouldElementBeIncluded(element))
            {
                _model.RemoveElement(element.Name);
            }
        }

        private bool ShouldElementBeIncluded(IElement element)
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

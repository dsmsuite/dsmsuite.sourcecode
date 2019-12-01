using System;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    class SplitProductAndTestElementsAction : Action
    {
        private const string ActionName = "Split test and product code in two separate linked hierarchies";
        private readonly IDataModel _model;
        private readonly SplitProductAndTestElementsSettings _splitProductAndTestElementsSettings;

        public SplitProductAndTestElementsAction(IDataModel model, SplitProductAndTestElementsSettings splitProductAndTestElementsSettings) :
            base(ActionName, splitProductAndTestElementsSettings.Enabled)
        {
            _model = model;
            _splitProductAndTestElementsSettings = splitProductAndTestElementsSettings;
        }

        protected override void ExecuteImpl()
        {
            int transformedElements = 0;

            IElement[] clonedElements = _model.Elements.ToArray(); // Because elements in collection change during iteration
            foreach (IElement element in clonedElements)
            {
                SplitProductAndTestElement(element);

                transformedElements++;
                Console.Write("\r progress elements={0}", transformedElements);
            }
            Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void SplitProductAndTestElement(IElement element)
        {
            Move(element, _splitProductAndTestElementsSettings.ProductElementIdentifier);
            Move(element, _splitProductAndTestElementsSettings.TestElementIdentifier);
        }

        private void Move(IElement element, string identifier)
        {
            string from = "." + identifier + ".";
            string to = ".";
            if (element.Name.Contains(from))
            {
                string elementName = element.Name;
                string newElementName = identifier + "." + element.Name.ReplaceIgnoreCase(from, to);

                _model.RenameElement(element, newElementName);

                string description = elementName + "->" + newElementName;
                AnalyzerLogger.LogTransformation(ActionName, description);
            }
        }
    }
}

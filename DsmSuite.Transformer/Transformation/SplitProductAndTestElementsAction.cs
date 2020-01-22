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
        private readonly IDsiModel _model;
        private readonly SplitProductAndTestElementsSettings _splitProductAndTestElementsSettings;

        public SplitProductAndTestElementsAction(IDsiModel model, SplitProductAndTestElementsSettings splitProductAndTestElementsSettings) :
            base(ActionName, splitProductAndTestElementsSettings.Enabled)
        {
            _model = model;
            _splitProductAndTestElementsSettings = splitProductAndTestElementsSettings;
        }

        protected override void ExecuteImpl(IProgress<ProgressInfo> progress)
        {
            int transformedElements = 0;

            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration
            foreach (IDsiElement element in clonedElements)
            {
                SplitProductAndTestElement(element);

                transformedElements++;
                //Console.Write("\r progress elements={0}", transformedElements);
            }
            //Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void SplitProductAndTestElement(IDsiElement element)
        {
            Move(element, _splitProductAndTestElementsSettings.ProductElementIdentifier);
            Move(element, _splitProductAndTestElementsSettings.TestElementIdentifier);
        }

        private void Move(IDsiElement element, string identifier)
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

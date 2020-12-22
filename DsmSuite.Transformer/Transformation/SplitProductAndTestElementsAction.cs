using System;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Transformer.Settings;

namespace DsmSuite.Transformer.Transformation
{
    class SplitProductAndTestElementsAction : Action
    {
        private const string ActionName = "Split test and product code in two separate linked hierarchies";
        private readonly IDsiModel _model;
        private readonly SplitProductAndTestElementsSettings _splitProductAndTestElementsSettings;

        public SplitProductAndTestElementsAction(IDsiModel model, SplitProductAndTestElementsSettings splitProductAndTestElementsSettings, IProgress<ProgressInfo> progress) :
            base(ActionName, splitProductAndTestElementsSettings.Enabled, progress)
        {
            _model = model;
            _splitProductAndTestElementsSettings = splitProductAndTestElementsSettings;
        }

        protected override void ExecuteImpl()
        {
            int totalElements = _model.GetElementCount();
            int transformedElements = 0;

            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration
            foreach (IDsiElement element in clonedElements)
            {
                SplitProductAndTestElement(element);

                transformedElements++;
                UpdateTransformationProgress(Name, transformedElements, totalElements);
            }
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
            }
        }
    }
}

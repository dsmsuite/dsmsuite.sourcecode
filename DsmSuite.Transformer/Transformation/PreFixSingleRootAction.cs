using System;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;

namespace DsmSuite.Transformer.Transformation
{
    public class PreFixSingleRootAction : Action
    {
        private const string ActionName = "Prefix all elements with root";
        private readonly IDsiModel _model;

        public PreFixSingleRootAction(IDsiModel model, bool enabled) :
            base(ActionName, enabled)
        {
            _model = model;
        }

        protected override void ExecuteImpl()
        {
            int transformedElements = 0;

            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration
            foreach (IDsiElement element in clonedElements)
            {
                PrefixElement(element);

                transformedElements++;
                Console.Write("\r progress elements={0}", transformedElements);
            }
        }

        private void PrefixElement(IDsiElement element)
        {
            string newElementName = "Root." + element.Name;
            _model.RenameElement(element, newElementName);
        }
    }
}

using System;
using System.Linq;
using DsmSuite.Analyzer.Data;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Transformer.Transformation
{
    public class MoveHeaderElementsAction : Action
    {
        private const string ActionName = "Move C/C++ header file element near to implementation file element";
        private readonly IDataModel _model;

        public MoveHeaderElementsAction(IDataModel model, bool enabled) :
            base(ActionName, enabled)
        {
            _model = model;
        }

        protected override void ExecuteImpl()
        {
            int transformedElements = 0;

            IElement[] clonedElements = _model.Elements.ToArray(); // Because elements in collection change during iteration
            foreach (IElement element in clonedElements)
            {
                MoveHeaderElement(element);

                transformedElements++;
                Console.Write("\r progress elements={0}", transformedElements);
            }
            Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void MoveHeaderElement(IElement element)
        {
            foreach (IRelation relation in element.Providers)
            {
                // Usual case where implementation file includes header file
                if (IsImplementation(relation.Consumer) &&
                    IsHeader(relation.Provider) &&
                    (GetName(relation.Consumer) == GetName(relation.Provider)))
                {
                    MergeHeaderAndImplementation(relation.Consumer, relation.Provider);
                }
            }
        }

        private void MergeHeaderAndImplementation(IElement consumer, IElement provider)
        {
            string consumerName = consumer.Name;
            string providerName = provider.Name;

            string newProviderName = GetNamespace(consumer) + "." +
                                     GetName(provider) + "." +
                                     GetExtension(provider);

            _model.RenameElement(provider, newProviderName);

            string description = "consumer=" + consumerName + " provider=" + providerName + "->" + newProviderName;
            AnalyzerLogger.LogTransformation(ActionName, description);
        }

        private bool IsHeader(IElement element)
        {
            return (GetExtension(element) == "hpp") || (GetExtension(element) == "h");
        }

        private bool IsImplementation(IElement element)
        {
            return (GetExtension(element) == "cpp") || (GetExtension(element) == "c");
        }

        private string GetExtension(IElement element)
        {
            string[] words = element.Name.Split('.');
            return words[words.Length - 1];
        }

        private string GetName(IElement element)
        {
            string[] words = element.Name.Split('.');
            return words[words.Length - 2];
        }

        private string GetNamespace(IElement element)
        {
            return element.Name.Substring(0, element.Name.Length - GetName(element).Length - GetExtension(element).Length - 2);
        }
    }
}

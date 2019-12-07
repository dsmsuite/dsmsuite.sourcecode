using System;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Transformer.Transformation
{
    public class MoveHeaderElementsAction : Action
    {
        private const string ActionName = "Move C/C++ header file element near to implementation file element";
        private readonly IDsiDataModel _model;

        public MoveHeaderElementsAction(IDsiDataModel model, bool enabled) :
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
                MoveHeaderElement(element);

                transformedElements++;
                Console.Write("\r progress elements={0}", transformedElements);
            }
            Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void MoveHeaderElement(IDsiElement element)
        {
            foreach (IDsiRelation relation in _model.GetProviderRelations(element))
            {
                IDsiElement consumer = _model.FindElement(relation.Consumer);
                IDsiElement provider = _model.FindElement(relation.Provider);

                // Usual case where implementation file includes header file
                if (IsImplementation(consumer) &&
                    IsHeader(provider) &&
                    (GetName(consumer) == GetName(provider)))
                {
                    MergeHeaderAndImplementation(consumer, provider);
                }
            }
        }

        private void MergeHeaderAndImplementation(IDsiElement consumer, IDsiElement provider)
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

        private bool IsHeader(IDsiElement element)
        {
            return (GetExtension(element) == "hpp") || (GetExtension(element) == "h");
        }

        private bool IsImplementation(IDsiElement element)
        {
            return (GetExtension(element) == "cpp") || (GetExtension(element) == "c");
        }

        private string GetExtension(IDsiElement element)
        {
            string[] words = element.Name.Split('.');
            return words[words.Length - 1];
        }

        private string GetName(IDsiElement element)
        {
            string[] words = element.Name.Split('.');
            return words[words.Length - 2];
        }

        private string GetNamespace(IDsiElement element)
        {
            return element.Name.Substring(0, element.Name.Length - GetName(element).Length - GetExtension(element).Length - 2);
        }
    }
}

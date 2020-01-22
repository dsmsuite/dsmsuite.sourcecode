using System;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Util;
using DsmSuite.Common.Util;

namespace DsmSuite.Transformer.Transformation
{
    public class MoveHeaderElementsAction : Action
    {
        private const string ActionName = "Move C/C++ header file element near to implementation file element";
        private readonly IDsiModel _model;

        public MoveHeaderElementsAction(IDsiModel model, bool enabled) :
            base(ActionName, enabled)
        {
            _model = model;
        }

        protected override void ExecuteImpl(IProgress<ProgressInfo> progress)
        {
            int transformedElements = 0;

            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration
            foreach (IDsiElement element in clonedElements)
            {
                MoveHeaderElement(element);

                transformedElements++;
                //Console.Write("\r progress elements={0}", transformedElements);
            }
            //Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void MoveHeaderElement(IDsiElement element)
        {
            foreach (IDsiRelation relation in _model.GetRelationsOfConsumer(element.Id))
            {
                IDsiElement consumer = _model.FindElementById(relation.ConsumerId);
                IDsiElement provider = _model.FindElementById(relation.ProviderId);

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

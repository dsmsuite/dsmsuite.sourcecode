using System;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Transformations.Transformation
{
    public class MoveHeaderToSourceFileDirectoryTransformationAction : TransformationAction
    {
        private const string ActionName = "Move C/C++ header file element near to implementation file element";
        private readonly IDsiModel _model;

        public MoveHeaderToSourceFileDirectoryTransformationAction(IDsiModel model, IProgress<ProgressInfo> progress) :
            base(ActionName, progress)
        {
            _model = model;
        }

        public override void Execute()
        {
            IDsiElement[] clonedElements = _model.GetElements().ToArray(); // Because elements in collection change during iteration

            int totalElements = _model.CurrentElementCount;
            int transformedElements = 0;
            foreach (IDsiElement element in clonedElements)
            {
                MoveHeaderElement(element);

                transformedElements++;
                UpdateTransformationProgress(Name, transformedElements, totalElements);
            }
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
            string newProviderName = GetNamespace(consumer) + "." +
                                     GetName(provider) + "." +
                                     GetExtension(provider);

            _model.RenameElement(provider, newProviderName);
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

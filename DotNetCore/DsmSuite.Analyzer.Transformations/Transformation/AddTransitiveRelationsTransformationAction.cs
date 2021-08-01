using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Transformations.Transformation
{
    public class AddTransitiveRelationsTransformationAction : TransformationAction
    {
        private const string ActionName = "Add transitive relations between elements";
        private readonly IDsiModel _model;
        private readonly Dictionary<string, HashSet<IDsiElement>> _directProviders;
        private readonly Dictionary<string, HashSet<IDsiElement>> _transitiveProviders;

        public AddTransitiveRelationsTransformationAction(IDsiModel model, IProgress<ProgressInfo> progress) :
           base(ActionName, progress)
        {
            _model = model;

            _directProviders = new Dictionary<string, HashSet<IDsiElement>>();
            _transitiveProviders = new Dictionary<string, HashSet<IDsiElement>>();
        }

        public override void Execute()
        {
            FindDirectProviders();

            int totalElements = _model.CurrentElementCount;
            int transformedElements = 0;
            foreach (IDsiElement consumer in _model.GetElements())
            {
                AddTransitiveRelations(consumer);

                transformedElements++;
                UpdateTransformationProgress(Name, transformedElements, totalElements);
            }
        }

        private void AddTransitiveRelations(IDsiElement consumer)
        {
            foreach (IDsiElement provider in GetProviders(consumer))
            {
                FindTransitiveProviders(consumer, provider);
            }
        }

        private void FindDirectProviders()
        {
            foreach (IDsiElement element in _model.GetElements())
            {
                string key = element.Name;

                _directProviders[key] = new HashSet<IDsiElement>();

                foreach (IDsiRelation providerRelation in _model.GetRelationsOfConsumer(element.Id))
                {
                    IDsiElement provider = _model.FindElementById(providerRelation.ProviderId);
                    _directProviders[key].Add(provider);
                }
            }
        }

        private ICollection<IDsiElement> GetProviders(IDsiElement consumer)
        {
            ICollection<IDsiElement> providers = new List<IDsiElement>();
            if (_directProviders.ContainsKey(consumer.Name))
            {
                providers = _directProviders[consumer.Name];
            }
            return providers;
        }

        private void FindTransitiveProviders(IDsiElement consumer, IDsiElement currentProvider)
        {
            foreach (IDsiElement transitiveProvider in GetProviders(currentProvider))
            {
                string key = consumer.Name;
                if (!_transitiveProviders.ContainsKey(consumer.Name))
                {
                    _transitiveProviders[key] = new HashSet<IDsiElement>();
                }

                if (!_transitiveProviders[key].Contains(transitiveProvider))
                {
                    _transitiveProviders[key].Add(transitiveProvider);
                    RegisterRelation(consumer, transitiveProvider);

                    FindTransitiveProviders(consumer, transitiveProvider);
                }
            }
        }

        private void RegisterRelation(IDsiElement consumer, IDsiElement provider)
        {
            if (consumer != null && provider != null)
            {
                string type = "transitive";
                _model.AddRelation(consumer.Name, provider.Name, type, 1, null);
            }
        }
    }
}

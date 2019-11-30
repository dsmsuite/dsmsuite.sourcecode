using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Data;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Transformer.Transformation
{
    public class AddTransitiveRelationsAction : Action
    {
        private const string ActionName = "Add transitive relations between elements";
        private readonly IDataModel _model;
        private readonly Dictionary<string, HashSet<IElement>> _directProviders;
        private readonly Dictionary<string, HashSet<IElement>> _transitiveProviders;

        public AddTransitiveRelationsAction(IDataModel model, bool enabled) :
           base(ActionName, enabled)
        {
            _model = model;

            _directProviders = new Dictionary<string, HashSet<IElement>>();
            _transitiveProviders = new Dictionary<string, HashSet<IElement>>();
        }

        protected override void ExecuteImpl()
        {
            FindDirectProviders();

            int transformedElements = 0;
            foreach (IElement consumer in _model.Elements)
            {
                AddTransitiveRelations(consumer);

                transformedElements++;
                Console.Write("\r progress elements={0}", transformedElements);
            }
            Console.WriteLine("\r progress elements={0}", transformedElements);
        }

        private void AddTransitiveRelations(IElement consumer)
        {
            foreach (IElement provider in GetProviders(consumer))
            {
                FindTransitiveProviders(consumer, provider);
            }
        }

        private void FindDirectProviders()
        {
            foreach (IElement element in _model.Elements)
            {
                string key = element.Name;

                _directProviders[key] = new HashSet<IElement>();

                foreach (IRelation providerRelation in _model.GetProviderRelations(element))
                {
                    _directProviders[key].Add(providerRelation.Provider);
                }
            }
        }

        private ICollection<IElement> GetProviders(IElement consumer)
        {
            ICollection<IElement> providers = new List<IElement>();
            if (_directProviders.ContainsKey(consumer.Name))
            {
                providers = _directProviders[consumer.Name];
            }
            return providers;
        }

        private void FindTransitiveProviders(IElement consumer, IElement currentProvider)
        {
            foreach (IElement transitiveProvider in GetProviders(currentProvider))
            {
                string key = consumer.Name;
                if (!_transitiveProviders.ContainsKey(consumer.Name))
                {
                    _transitiveProviders[key] = new HashSet<IElement>();
                }

                if (!_transitiveProviders[key].Contains(transitiveProvider))
                {
                    _transitiveProviders[key].Add(transitiveProvider);
                    RegisterRelation(consumer, transitiveProvider);

                    FindTransitiveProviders(consumer, transitiveProvider);
                }
            }
        }

        private void RegisterRelation(IElement consumer, IElement provider)
        {
            if (consumer != null && provider != null)
            {
                string type = "transitive";
                _model.AddRelation(consumer.Name, provider.Name, type, 1, "transformer");

                string description = "consumer=" + consumer.Name + " provider=" + provider.Name;
                AnalyzerLogger.LogTransformation(ActionName, description);
            }
        }
    }
}

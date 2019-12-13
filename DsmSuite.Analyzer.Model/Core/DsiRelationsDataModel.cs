using System.Linq;
using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Analyzer.Model.Core
{
    public class DsiRelationsDataModel
    {
        private readonly DsiElementsDataModel _elementsDataModel;
        private readonly Dictionary<int, Dictionary<int, Dictionary<string, DsiRelation>>> _relationsByConsumerId;
        private readonly Dictionary<string, int> _relationTypeCount;
        private int _relationCount;

        public DsiRelationsDataModel(DsiElementsDataModel elementsDataModel)
        {
            _elementsDataModel = elementsDataModel;
            _relationsByConsumerId = new Dictionary<int, Dictionary<int, Dictionary<string, DsiRelation>>>();
            _relationTypeCount = new Dictionary<string, int>();
        }

        public void Clear()
        {
            _relationsByConsumerId.Clear();
            _relationTypeCount.Clear();
            _relationCount = 0;
        }

        public IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            Logger.LogDataModelMessage("Import relation consumerId={consumerId} providerId={providerId} type={type} weight={weight}");

            _relationCount++;

            DsiRelation relation = null;

            IDsiElement consumer = _elementsDataModel.FindElementById(consumerId);
            IDsiElement provider = _elementsDataModel.FindElementById(providerId);

            if ((consumer != null) && (provider != null))
            {
                relation = AddOrUpdateRelation(consumer.Id, provider.Id, type, weight);
            }
            else
            {
                AnalyzerLogger.LogDataModelRelationNotResolved(consumerId.ToString(), providerId.ToString());
            }
            return relation;
        }
        
        public IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, string context)
        {
            Logger.LogDataModelMessage("Add relation consumerName={consumerName} providerName={providerName} type={type} weight={weight}");

            DsiRelation relation = null;

            _relationCount++;

            IDsiElement consumer = _elementsDataModel.FindElementByName(consumerName);
            IDsiElement provider = _elementsDataModel.FindElementByName(providerName);

            if ((consumer != null) && (provider != null))
            {
                relation = AddOrUpdateRelation(consumer.Id, provider.Id, type, weight);
            }
            else
            {
                AnalyzerLogger.LogDataModelRelationNotResolved(consumerName, providerName);
            }

            return relation;
        }

        private DsiRelation AddOrUpdateRelation(int consumerId, int providerId, string type, int weight)
        {
            Dictionary<string, DsiRelation> relations = GetRelations(consumerId, providerId);

            IncrementRelationTypeCount(type);

            if (relations.ContainsKey(type))
            {
                _relationCount--; // Revert previous increment when relation exists and just weight increased
                relations[type].Weight += weight;
            }
            else
            {
                relations[type] = new DsiRelation(consumerId, providerId, type, weight);
            }

            return relations[type];
        }

        public void SkipRelation(string consumerName, string providerName, string type, string context)
        {
            Logger.LogDataModelMessage("Skip relation consumerName={consumerName} providerName={providerName} type={type} weight={weight}");

            AnalyzerLogger.LogDataModelRelationNotResolved(consumerName, providerName);

            _relationCount++;
        }

        public ICollection<string> GetRelationTypes()
        {
            return _relationTypeCount.Keys;
        }

        public int GetRelationTypeCount(string type)
        {
            if (_relationTypeCount.ContainsKey(type))
            {
                return _relationTypeCount[type];
            }
            else
            {
                return 0;
            }
        }

        public ICollection<IDsiRelation> GetRelationsOfConsumer(int consumerId)
        {
            List<IDsiRelation> relations = new List<IDsiRelation>();
            if (_relationsByConsumerId.ContainsKey(consumerId))
            {
                foreach (Dictionary<string, DsiRelation> relations2 in _relationsByConsumerId[consumerId].Values)
                {
                    relations.AddRange(relations2.Values);
                }
            }
            return relations;
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            List<IDsiRelation> relations = new List<IDsiRelation>();
            foreach (Dictionary<int, Dictionary<string, DsiRelation>> consumerRelations in _relationsByConsumerId.Values)
            {
                foreach (Dictionary<string, DsiRelation> relations2 in consumerRelations.Values)
                {
                    relations.AddRange(relations2.Values);
                }
            }
            return relations;
        }

        public bool DoesRelationExist(int consumerId, int providerId)
        {
            return _relationsByConsumerId.ContainsKey(consumerId) &&
                   _relationsByConsumerId[consumerId].ContainsKey(providerId);
        }

        public int TotalRelationCount => _relationCount;

        public int ResolvedRelationCount
        {
            get
            {
                return GetRelations().Count();
            }
        }

        public double ResolvedRelationPercentage
        {
            get
            {
                double resolvedRelationPercentage = 0.0;
                if (TotalRelationCount > 0)
                {
                    resolvedRelationPercentage = (ResolvedRelationCount * 100.0) / TotalRelationCount;
                }
                return resolvedRelationPercentage;
            }
        }

        //public void RemoveRelationsForRemovedElements()
        //{
        //    foreach (IDsiElement element in _elementsDataModel.GetElements())
        //    {
        //        if (_relationsByConsumerId.ContainsKey(element.Id))
        //        {
        //            IDsiRelation[] relations = _relationsByConsumerId[element.Id].ToArray();

        //            foreach (IDsiRelation relation in relations)
        //            {
        //                if ((_elementsDataModel.FindElementById(relation.ConsumerId) == null) ||
        //                    (_elementsDataModel.FindElementById(relation.ProviderId) == null))
        //                {
        //                    _relationsByConsumerId[element.Id].Remove(relation);
        //                }
        //            }
        //        }
        //    }
        //}

        private void IncrementRelationTypeCount(string type)
        {
            if (!_relationTypeCount.ContainsKey(type))
            {
                _relationTypeCount[type] = 0;
            }
            _relationTypeCount[type]++;
        }

        private Dictionary<string, DsiRelation> GetRelations(int consumerId, int providerId)
        {
            if (!_relationsByConsumerId.ContainsKey(consumerId))
            {
                _relationsByConsumerId[consumerId] = new Dictionary<int, Dictionary<string, DsiRelation>>();
            }

            if (!_relationsByConsumerId[consumerId].ContainsKey(providerId))
            {
                _relationsByConsumerId[consumerId][providerId] = new Dictionary<string, DsiRelation>();
            }

            return _relationsByConsumerId[consumerId][providerId];
        }
    }
}

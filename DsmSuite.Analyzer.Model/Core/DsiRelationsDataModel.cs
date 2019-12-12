using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Analyzer.Model.Core
{
    public class DsiRelationsDataModel
    {
        private readonly DsiElementsDataModel _elementsDataModel;
        private readonly Dictionary<int, List<IDsiRelation>> _relationsByConsumerId;
        private readonly Dictionary<string, int> _relationTypeCount;
        private int _relationCount;

        public DsiRelationsDataModel(DsiElementsDataModel elementsDataModel)
        {
            _elementsDataModel = elementsDataModel;
            _relationsByConsumerId = new Dictionary<int, List<IDsiRelation>>();
            _relationTypeCount = new Dictionary<string, int>();
        }

        public void Clear()
        {
            _relationsByConsumerId.Clear();
            _relationTypeCount.Clear();
            _relationCount = 0;
        }

        public void ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            Logger.LogDataModelMessage("Import relation consumerId={consumerId} providerId={providerId} type={type} weight={weight}");

            _relationCount++;

            IncrementRelationTypeCount(type);

            if (!_relationsByConsumerId.ContainsKey(consumerId))
            {
                _relationsByConsumerId[consumerId] = new List<IDsiRelation>();
            }
            DsiRelation relation = new DsiRelation(consumerId, providerId, type, weight);
            _relationsByConsumerId[relation.ConsumerId].Add(relation);
        }

        public IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, string context)
        {
            Logger.LogDataModelMessage("Add relation consumerName={consumerName} providerName={providerName} type={type} weight={weight}");

            _relationCount++;

            IDsiElement consumer = _elementsDataModel.FindElementByName(consumerName);
            IDsiElement provider = _elementsDataModel.FindElementByName(providerName);
            IDsiRelation relation = null;

            if (consumer != null && provider != null)
            {
                IncrementRelationTypeCount(type);

                relation = new DsiRelation(consumer.Id, provider.Id, type, weight);
                if (!_relationsByConsumerId.ContainsKey(consumer.Id))
                {
                    _relationsByConsumerId[consumer.Id] = new List<IDsiRelation>();
                }
                _relationsByConsumerId[consumer.Id].Add(relation);
            }
            else
            {
                AnalyzerLogger.LogDataModelRelationNotResolved(consumerName, providerName);
            }

            return relation;
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
            if (_relationsByConsumerId.ContainsKey(consumerId))
            {
                return _relationsByConsumerId[consumerId];
            }
            else
            {
                return new List<IDsiRelation>();
            }
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            List<IDsiRelation> relations = new List<IDsiRelation>();
            foreach (List<IDsiRelation> consumerRelations in _relationsByConsumerId.Values)
            {
                relations.AddRange(consumerRelations);
            }
            return relations;
        }

        public bool DoesRelationExist(int consumerId, int providerId)
        {
            bool doesRelationExist = false;

            if (_relationsByConsumerId.ContainsKey(consumerId))
            {
                foreach (IDsiRelation relation in _relationsByConsumerId[consumerId])
                {
                    if (relation.ProviderId == providerId)
                    {
                        doesRelationExist = true;
                    }
                }
            }

            return doesRelationExist;
        }

        public int TotalRelationCount => _relationCount;

        public int ResolvedRelationCount
        {
            get
            {
                int count = 0;

                foreach (IDsiElement element in _elementsDataModel.GetElements())
                {
                    if (_relationsByConsumerId.ContainsKey(element.Id))
                    {
                        count += _relationsByConsumerId[element.Id].Count;
                    }
                }
                return count;
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

        public void RemoveRelationsForRemovedElements()
        {
            foreach (IDsiElement element in _elementsDataModel.GetElements())
            {
                if (_relationsByConsumerId.ContainsKey(element.Id))
                {
                    IDsiRelation[] relations = _relationsByConsumerId[element.Id].ToArray();

                    foreach (IDsiRelation relation in relations)
                    {
                        if ((_elementsDataModel.FindElementById(relation.ConsumerId) == null) ||
                            (_elementsDataModel.FindElementById(relation.ProviderId) == null))
                        {
                            _relationsByConsumerId[element.Id].Remove(relation);
                        }
                    }
                }
            }
        }

        private void IncrementRelationTypeCount(string type)
        {
            if (!_relationTypeCount.ContainsKey(type))
            {
                _relationTypeCount[type] = 0;
            }
            _relationTypeCount[type]++;
        }
    }
}

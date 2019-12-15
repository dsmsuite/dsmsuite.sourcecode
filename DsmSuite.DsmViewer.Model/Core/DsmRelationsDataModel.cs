using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmRelationsDataModel
    {
        private readonly DsmElementsDataModel _elementsDataModel;
        private readonly Dictionary<int /*relationId*/, IDsmRelation> _relationsById;
        private readonly Dictionary<int /*providerId*/, Dictionary<int /*consumerId*/, IDsmRelation>> _relationsByProvider;
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, IDsmRelation>> _relationsByConsumer;
        private readonly Dictionary<int /*relationId*/, IDsmRelation> _deletedRelationsById;

        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, int /*weight*/>> _weights;
        private int _lastRelationId;

        public DsmRelationsDataModel(DsmElementsDataModel elementsDataModel)
        {
            _elementsDataModel = elementsDataModel;
            _elementsDataModel.ElementRemoved += OnElementRemoved;
            _elementsDataModel.ElementUnremoved += OnElementUnremoved;

            _relationsById = new Dictionary<int, IDsmRelation>();
            _relationsByProvider = new Dictionary<int, Dictionary<int, IDsmRelation>>();
            _relationsByConsumer = new Dictionary<int, Dictionary<int, IDsmRelation>>();
            _deletedRelationsById = new Dictionary<int, IDsmRelation>();
            _lastRelationId = 0;
            _weights = new Dictionary<int, Dictionary<int, int>>();
        }
        
        public void Clear()
        {
            _relationsById.Clear();
            _relationsByProvider.Clear();
            _relationsByConsumer.Clear();
            _lastRelationId = 0;

            _weights.Clear();
        }

        public IDsmRelation ImportRelation(int relationId, int consumerId, int providerId, string type, int weight)
        {
            Logger.LogDataModelMessage("Import relation relationId={relationId} consumerId={consumerId} providerId={providerId} type={type} weight={weight}");

            if (relationId > _lastRelationId)
            {
                _lastRelationId = relationId;
            }

            DsmRelation relation = null;
            if (consumerId != providerId)
            {
                relation = new DsmRelation(relationId, consumerId, providerId, type, weight);
                RegisterRelation(relation);
            }
            return relation;
        }

        public IDsmRelation AddRelation(int consumerId, int providerId, string type, int weight)
        {
            Logger.LogDataModelMessage("Add relation consumerId={consumerId} providerId={providerId} type={type} weight={weight}");

            DsmRelation relation = null;
            if (consumerId != providerId)
            {
                _lastRelationId++;
                relation = new DsmRelation(_lastRelationId, consumerId, providerId, type, weight);
                RegisterRelation(relation);
            }
            return relation;
        }

        public void EditRelation(IDsmRelation relation, string type, int weight)
        {
            DsmRelation editedRelation = relation as DsmRelation;
            if (editedRelation != null)
            {
                UnregisterRelation(relation);

                editedRelation.Type = type;
                editedRelation.Weight = weight;

                RegisterRelation(relation);
            }
        }

        public void RemoveRelation(IDsmRelation relation)
        {
            if (relation != null)
            {
                UnregisterRelation(relation);
            }
        }

        public void UnremoveRelation(IDsmRelation relation)
        {
            if (relation != null)
            {
                RegisterRelation(relation);
            }
        }

        public int GetDependencyWeight(int consumerId, int providerId)
        {
            int weight = 0;
            if ((consumerId != providerId) && _weights.ContainsKey(consumerId) && _weights[consumerId].ContainsKey(providerId))
            {
                weight = _weights[consumerId][providerId];
            }
            return weight;
        }

        public bool IsCyclicDependency(int consumerId, int providerId)
        {
            return (GetDependencyWeight(consumerId, providerId) > 0) &&
                   (GetDependencyWeight(providerId, consumerId) > 0);
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            IList<IDsmRelation> relations = new List<IDsmRelation>();
            HashSet<int> consumerIds = GetIdsOfElementAndItsChidren(consumer);
            HashSet<int> providerIds = GetIdsOfElementAndItsChidren(provider);
            foreach (int consumerId in consumerIds)
            {
                foreach (int providerId in providerIds)
                {
                    if (_relationsByConsumer.ContainsKey(consumerId) && _relationsByConsumer[consumerId].ContainsKey(providerId))
                    {
                        relations.Add(_relationsByConsumer[consumerId][providerId]);
                    }
                }
            }
            return relations;
        }

        public IEnumerable<IDsmRelation> FindRelationsWhereElementHasProviderRole(IDsmElement element)
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            HashSet<int> providerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int providerId in providerIds)
            {
                if (_relationsByProvider.ContainsKey(providerId))
                {
                    foreach (IDsmRelation relation in _relationsByProvider[providerId].Values)
                    {
                        if (!providerIds.Contains(relation.ConsumerId))
                        {
                            relations.Add(relation);
                        }
                    }
                }
            }
            return relations;
        }

        public IEnumerable<IDsmRelation> FindRelationsWhereElementHasConsumerRole(IDsmElement element)
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            HashSet<int> consumerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int consumerId in consumerIds)
            {
                if (_relationsByConsumer.ContainsKey(consumerId))
                {
                    foreach (IDsmRelation relation in _relationsByConsumer[consumerId].Values)
                    {
                        if (!consumerIds.Contains(relation.ProviderId))
                        {
                            relations.Add(relation);
                        }
                    }
                }
            }
            return relations;
        }

        public IEnumerable<IDsmRelation> GetRelations()
        {
            return _relationsById.Values.OrderBy(x => x.Id);
        }

        public int GetRelationCount()
        {
            return _relationsById.Values.Count;
        }

        private void OnElementRemoved(object sender, IDsmElement element)
        {
            HashSet<int> elementIds = GetIdsOfElementAndItsChidren(element);

            List<IDsmRelation> toBeRemovedRelations = new List<IDsmRelation>();
             
            foreach(IDsmRelation relation in _relationsById.Values)
            {
                if (elementIds.Contains(relation.ConsumerId) ||
                    elementIds.Contains(relation.ProviderId))
                {
                    toBeRemovedRelations.Add(relation);
                }
            }

            foreach(IDsmRelation relation in toBeRemovedRelations)
            {
                UnregisterRelation(relation);
            }
        }

        private void OnElementUnremoved(object sender, IDsmElement element)
        {
            HashSet<int> elementIds = GetIdsOfElementAndItsChidren(element);

            List<IDsmRelation> toBeUnremovedRelations = new List<IDsmRelation>();

            foreach (IDsmRelation relation in _deletedRelationsById.Values)
            {
                if (elementIds.Contains(relation.ConsumerId) ||
                    elementIds.Contains(relation.ProviderId))
                {
                    toBeUnremovedRelations.Add(relation);
                }
            }

            foreach (IDsmRelation relation in toBeUnremovedRelations)
            {
                RegisterRelation(relation);
            }
        }
       
        private HashSet<int> GetIdsOfElementAndItsChidren(IDsmElement element)
        {
            HashSet<int> ids = new HashSet<int>();
            GetIdsOfElementAndItsChidren(element, ids);
            return ids;
        }

        private void GetIdsOfElementAndItsChidren(IDsmElement element, HashSet<int> ids)
        {
            ids.Add(element.Id);

            foreach (IDsmElement child in element.Children)
            {
                GetIdsOfElementAndItsChidren(child, ids);
            }
        }

        private void RegisterRelation(IDsmRelation relation)
        {
            _relationsById[relation.Id] = relation;

            if (_deletedRelationsById.ContainsKey(relation.Id))
            {
                _deletedRelationsById.Remove(relation.Id);
            }

            if (!_relationsByProvider.ContainsKey(relation.ProviderId))
            {
                _relationsByProvider[relation.ProviderId] = new Dictionary<int, IDsmRelation>();
            }
            _relationsByProvider[relation.ProviderId][relation.ConsumerId] = relation;

            if (!_relationsByConsumer.ContainsKey(relation.ConsumerId))
            {
                _relationsByConsumer[relation.ConsumerId] = new Dictionary<int, IDsmRelation>();
            }
            _relationsByConsumer[relation.ConsumerId][relation.ProviderId] = relation;

            UpdateWeights(relation, AddWeight);
        }

        private void UnregisterRelation(IDsmRelation relation)
        {
            _relationsById.Remove(relation.Id);

            _deletedRelationsById[relation.Id] = relation;

            if (!_relationsByProvider.ContainsKey(relation.ProviderId))
            {
                _relationsByProvider[relation.ProviderId].Remove(relation.ConsumerId);
            }

            if (!_relationsByConsumer.ContainsKey(relation.ConsumerId))
            {
                _relationsByConsumer[relation.ConsumerId].Remove(relation.ProviderId);
            }

            UpdateWeights(relation, RemoveWeight);
        }

        private delegate void ModifyWeight(int consumerId, int providerId, int weight);

        private void UpdateWeights(IDsmRelation relation, ModifyWeight modifyWeight)
        {
            int consumerId = relation.ConsumerId;
            int providerId = relation.ProviderId;

            IDsmElement currentConsumer = _elementsDataModel.FindElementById(consumerId);
            while (currentConsumer != null)
            {
                IDsmElement currentProvider = _elementsDataModel.FindElementById(providerId);
                while (currentProvider != null)
                {
                    modifyWeight(currentConsumer.Id, currentProvider.Id, relation.Weight);
                    currentProvider = currentProvider.Parent;
                }
                currentConsumer = currentConsumer.Parent;
            }
        }

        private void AddWeight(int consumerId, int providerId, int weight)
        {
            if (!_weights.ContainsKey(consumerId))
            {
                _weights[consumerId] = new Dictionary<int, int>();
            }

            int oldWeight = 0;
            if (_weights[consumerId].ContainsKey(providerId))
            {
                oldWeight = _weights[consumerId][providerId];
            }
            int newWeight = oldWeight + weight;
            _weights[consumerId][providerId] = newWeight;
        }

        private void RemoveWeight(int consumerId, int providerId, int weight)
        {
            if (_weights.ContainsKey(consumerId) && _weights[consumerId].ContainsKey(providerId))
            {
                int currentWeight = _weights[consumerId][providerId];

                if (currentWeight >= weight)
                {
                    _weights[consumerId][providerId] -= weight;
                }
                else
                {
                    Logger.LogError($"Weight defined between consumerId={consumerId} and providerId={providerId} too low currentWeight={currentWeight} weight={weight}");
                }

                if (_weights[consumerId][providerId] == 0)
                {
                    _weights[consumerId].Remove(providerId);

                    if (_weights[consumerId].Count == 0)
                    {
                        _weights.Remove(consumerId);
                    }
                }
            }
            else
            {
                Logger.LogError($"No weight defined between consumerId={consumerId} and providerId={providerId}");
            }
        }
    }
}

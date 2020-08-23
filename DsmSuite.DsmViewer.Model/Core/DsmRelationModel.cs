using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmRelationModel : IDsmRelationModelFileCallback
    {
        private readonly DsmElementModel _elementsDataModel;
        private readonly Dictionary<int /*relationId*/, DsmRelation> _relationsById;
        private readonly Dictionary<int /*providerId*/, Dictionary<int /*consumerId*/, Dictionary<string /*type*/, DsmRelation>>> _relationsByProvider;
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, Dictionary<string /*type*/, DsmRelation>>> _relationsByConsumer;
        private readonly Dictionary<int /*relationId*/, DsmRelation> _deletedRelationsById;

        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, int /*weight*/>> _weights;
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, int /*weight*/>> _directWeights;
        private int _lastRelationId;

        public DsmRelationModel(DsmElementModel elementsDataModel)
        {
            _elementsDataModel = elementsDataModel;
            _elementsDataModel.UnregisterElementRelations += OnUnregisterElementRelations;
            _elementsDataModel.ReregisterElementRelations += OnReregisterElementRelations;

            _elementsDataModel.BeforeElementChangeParent += OnBeforeElementChangeParent;
            _elementsDataModel.AfterElementChangeParent += OnAfterElementChangeParent;

            _relationsById = new Dictionary<int, DsmRelation>();
            _relationsByProvider = new Dictionary<int, Dictionary<int, Dictionary<string, DsmRelation>>>();
            _relationsByConsumer = new Dictionary<int, Dictionary<int, Dictionary<string, DsmRelation>>>();
            _deletedRelationsById = new Dictionary<int, DsmRelation>();
            _lastRelationId = 0;
            _weights = new Dictionary<int, Dictionary<int, int>>();
            _directWeights = new Dictionary<int, Dictionary<int, int>>();
        }
        
        public void Clear()
        {
            _relationsById.Clear();
            _relationsByProvider.Clear();
            _relationsByConsumer.Clear();
            _deletedRelationsById.Clear();

            _lastRelationId = 0;

            _weights.Clear();
            _directWeights.Clear();
        }

        public void ClearHistory()
        {
            _deletedRelationsById.Clear();
        }

        public IDsmRelation ImportRelation(int relationId, int consumerId, int providerId, string type, int weight, bool deleted)
        {
            Logger.LogDataModelMessage("Import relation relationId={relationId} consumerId={consumerId} providerId={providerId} type={type} weight={weight}");

            if (relationId > _lastRelationId)
            {
                _lastRelationId = relationId;
            }

            DsmRelation relation = null;
            if (consumerId != providerId)
            {
                IDsmElement consumer = _elementsDataModel.FindElementById(consumerId);
                IDsmElement provider = _elementsDataModel.FindElementById(providerId);
                if ((consumer != null) && (provider != null))
                {
                    relation = new DsmRelation(relationId, consumer, provider, type, weight) { IsDeleted = deleted };
                    if (deleted)
                    {
                        UnregisterRelation(relation);
                    }
                    else
                    {
                        RegisterRelation(relation);
                    }
                }

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
                IDsmElement consumer = _elementsDataModel.FindElementById(consumerId);
                IDsmElement provider = _elementsDataModel.FindElementById(providerId);
                if ((consumer != null) && (provider != null))
                {
                    relation = new DsmRelation(_lastRelationId, consumer, provider, type, weight) { IsDeleted = false };
                    RegisterRelation(relation);
                }
            }
            return relation;
        }

        public void ChangeRelationType(IDsmRelation relation, string type)
        {
            DsmRelation changedRelation = relation as DsmRelation;
            if (changedRelation != null)
            {
                UnregisterRelation(changedRelation);

                changedRelation.Type = type;

                RegisterRelation(changedRelation);
            }
        }

        public void ChangeRelationWeight(IDsmRelation relation, int weight)
        {
            DsmRelation changedRelation = relation as DsmRelation;
            if (changedRelation != null)
            {
                UnregisterRelation(changedRelation);

                changedRelation.Weight = weight;

                RegisterRelation(changedRelation);
            }
        }

        public void RemoveRelation(int relationId)
        {
            if (_relationsById.ContainsKey(relationId))
            {
                DsmRelation relation = _relationsById[relationId];
                if (relation != null)
                {
                    UnregisterRelation(relation);
                }
            }
        }

        public void UnremoveRelation(int relationId)
        {
            if (_deletedRelationsById.ContainsKey(relationId))
            {
                DsmRelation relation = _deletedRelationsById[relationId];
                if (relation != null)
                {
                    RegisterRelation(relation);
                }
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

        public int GetDirectDependencyWeight(int consumerId, int providerId)
        {
            int weight = 0;
            if ((consumerId != providerId) && _directWeights.ContainsKey(consumerId) && _directWeights[consumerId].ContainsKey(providerId))
            {
                weight = _directWeights[consumerId][providerId];
            }
            return weight;
        }

        public CycleType IsCyclicDependency(int consumerId, int providerId)
        {
            if ((GetDirectDependencyWeight(consumerId, providerId) > 0) &&
                (GetDirectDependencyWeight(providerId, consumerId) > 0))
            {
                return CycleType.System;
            }
            else if ((GetDependencyWeight(consumerId, providerId) > 0) &&
                     (GetDependencyWeight(providerId, consumerId) > 0))
            {
                return CycleType.Hierarchical;
            }
            else
            {
                return CycleType.None;
            }
        }

        public IDsmRelation GetRelationById(int id)
        {
            return _relationsById.ContainsKey(id) ? _relationsById[id] : null;
        }

        public IDsmRelation GetDeletedRelationById(int id)
        {
            return _deletedRelationsById.ContainsKey(id) ? _deletedRelationsById[id] : null;
        }

        public IDsmRelation FindRelation(int consumerId, int providerId, string type)
        {
            IDsmRelation relation = null;
            if (_relationsByConsumer.ContainsKey(consumerId) &&
                _relationsByConsumer[consumerId].ContainsKey(providerId) &&
                _relationsByConsumer[consumerId][providerId].ContainsKey(type))
            {
                relation = _relationsByConsumer[consumerId][providerId][type];
            }
            return relation;
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
                        foreach (IDsmRelation relation in _relationsByConsumer[consumerId][providerId].Values)
                        {
                            if (!relation.IsDeleted)
                            {
                                relations.Add(relation);
                            }
                        }
                    }
                }
            }
            return relations;
        }

        public IEnumerable<IDsmRelation> FindIngoingRelations(IDsmElement element)
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            HashSet<int> providerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int providerId in providerIds)
            {
                if (_relationsByProvider.ContainsKey(providerId))
                {
                    foreach (Dictionary<string, DsmRelation> relationPerType in _relationsByProvider[providerId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (!providerIds.Contains(relation.Consumer.Id))
                            {
                                if (!relation.IsDeleted)
                                {
                                    relations.Add(relation);
                                }
                            }
                        }
                    }
                }
            }
            return relations;
        }

        public IEnumerable<IDsmRelation> FindOutgoingRelations(IDsmElement element)
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            HashSet<int> consumerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int consumerId in consumerIds)
            {
                if (_relationsByConsumer.ContainsKey(consumerId))
                {
                    foreach (Dictionary<string, DsmRelation> relationPerType in _relationsByConsumer[consumerId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (!consumerIds.Contains(relation.Provider.Id))
                            {
                                if (!relation.IsDeleted)
                                {
                                    relations.Add(relation);
                                }
                            }
                        }
                    }
                }
            }
            return relations;
        }

        public IEnumerable<IDsmRelation> FindInternalRelations(IDsmElement element)
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            HashSet<int> consumerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int consumerId in consumerIds)
            {
                if (_relationsByConsumer.ContainsKey(consumerId))
                {
                    foreach (Dictionary<string, DsmRelation> relationPerType in _relationsByConsumer[consumerId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (consumerIds.Contains(relation.Provider.Id))
                            {
                                if (!relation.IsDeleted)
                                {
                                    relations.Add(relation);
                                }
                            }
                        }
                    }
                }
            }
            return relations;
        }

        public int GetHierarchicalCycleCount(IDsmElement element)
        {
            int cyclesCount = 0;
            CountCycles(element, CycleType.Hierarchical, ref cyclesCount);
            return cyclesCount / 2;
        }

        public int GetSystemCycleCount(IDsmElement element)
        {
            int cyclesCount = 0;
            CountCycles(element, CycleType.System, ref cyclesCount);
            return cyclesCount / 2;
        }

        public IEnumerable<IDsmRelation> GetRelations()
        {
            return _relationsById.Values;
        }

        public int GetRelationCount()
        {
            return _relationsById.Values.Count;
        }

        public IEnumerable<IDsmRelation> GetExportedRelations()
        {
            List<IDsmRelation> exportedRelations = new List<IDsmRelation>();
            exportedRelations.AddRange(_relationsById.Values);
            exportedRelations.AddRange(_deletedRelationsById.Values);
            return exportedRelations.OrderBy(x => x.Id);
        }

        public int GetExportedRelationCount()
        {
            return _relationsById.Values.Count + _deletedRelationsById.Values.Count;
        }

        private void OnUnregisterElementRelations(object sender, IDsmElement element)
        {
            List<DsmRelation> toBeRelationsUnregistered = new List<DsmRelation>();

            foreach (DsmRelation relation in _relationsById.Values)
            {
                if ((element.Id == relation.Consumer.Id) ||
                    (element.Id == relation.Provider.Id))
                {
                    toBeRelationsUnregistered.Add(relation);
                }
            }

            foreach (DsmRelation relation in toBeRelationsUnregistered)
            {
                UnregisterRelation(relation);
            }
        }

        private void OnReregisterElementRelations(object sender, IDsmElement element)
        {
            List<DsmRelation> toBeRelationsReregistered = new List<DsmRelation>();

            foreach (DsmRelation relation in _deletedRelationsById.Values)
            {
                if ((element.Id == relation.Consumer.Id) ||
                    (element.Id == relation.Provider.Id))
                {
                    toBeRelationsReregistered.Add(relation);
                }
            }

            foreach (DsmRelation relation in toBeRelationsReregistered)
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
            if (!element.IsDeleted)
            {
                ids.Add(element.Id);
            }

            foreach (IDsmElement child in element.Children)
            {
                GetIdsOfElementAndItsChidren(child, ids);
            }
        }

        private void RegisterRelation(DsmRelation relation)
        {
            relation.IsDeleted = false;
            _relationsById[relation.Id] = relation;

            if (_deletedRelationsById.ContainsKey(relation.Id))
            {
                _deletedRelationsById.Remove(relation.Id);
            }

            if (!_relationsByProvider.ContainsKey(relation.Provider.Id))
            {
                _relationsByProvider[relation.Provider.Id] = new Dictionary<int, Dictionary<string, DsmRelation>>();
            }

            if (!_relationsByProvider[relation.Provider.Id].ContainsKey(relation.Consumer.Id))
            {
                _relationsByProvider[relation.Provider.Id][relation.Consumer.Id] = new Dictionary<string, DsmRelation>();
            }

            _relationsByProvider[relation.Provider.Id][relation.Consumer.Id][relation.Type] = relation;

            if (!_relationsByConsumer.ContainsKey(relation.Consumer.Id))
            {
                _relationsByConsumer[relation.Consumer.Id] = new Dictionary<int, Dictionary<string, DsmRelation>>();
            }

            if (!_relationsByConsumer[relation.Consumer.Id].ContainsKey(relation.Provider.Id))
            {
                _relationsByConsumer[relation.Consumer.Id][relation.Provider.Id] = new Dictionary<string, DsmRelation>();
            }

            _relationsByConsumer[relation.Consumer.Id][relation.Provider.Id][relation.Type] = relation;

            if (!_directWeights.ContainsKey(relation.Consumer.Id))
            {
                _directWeights[relation.Consumer.Id] = new Dictionary<int, int>();
            }

            if (_directWeights[relation.Consumer.Id].ContainsKey(relation.Provider.Id))
            {
                _directWeights[relation.Consumer.Id][relation.Provider.Id] += relation.Weight;
            }
            else
            {
                _directWeights[relation.Consumer.Id][relation.Provider.Id] = relation.Weight;
            }

            AddWeights(relation);
        }

        private void UnregisterRelation(DsmRelation relation)
        {
            relation.IsDeleted = true;
            _relationsById.Remove(relation.Id);

            if (_relationsByProvider.ContainsKey(relation.Provider.Id) &&
                _relationsByProvider[relation.Provider.Id].ContainsKey(relation.Consumer.Id) &&
                _relationsByProvider[relation.Provider.Id][relation.Consumer.Id].ContainsKey(relation.Type))
            {
                _relationsByProvider[relation.Provider.Id][relation.Consumer.Id].Remove(relation.Type);
            }

            if (_relationsByConsumer.ContainsKey(relation.Consumer.Id) &&
                _relationsByConsumer[relation.Consumer.Id].ContainsKey(relation.Provider.Id) &&
                _relationsByConsumer[relation.Consumer.Id][relation.Provider.Id].ContainsKey(relation.Type))
            {
                _relationsByConsumer[relation.Consumer.Id][relation.Provider.Id].Remove(relation.Type);
            }

            _deletedRelationsById[relation.Id] = relation;

            if (_directWeights.ContainsKey(relation.Consumer.Id) && _directWeights[relation.Consumer.Id].ContainsKey(relation.Provider.Id))
            {
                _directWeights[relation.Consumer.Id][relation.Provider.Id] -= relation.Weight;
            }

            RemoveWeights(relation);
        }

        private IEnumerable<DsmRelation> FindRelations(IDsmElement element)
        {
            List<DsmRelation> relations = new List<DsmRelation>();
            FindRelations(element, ref relations);
            return relations;
        }

        private void FindRelations(IDsmElement element, ref List<DsmRelation> foundRelations)
        {
            if (_relationsByConsumer.ContainsKey(element.Id))
            {
                foreach (Dictionary<string, DsmRelation> relationsByType in _relationsByConsumer[element.Id].Values)
                {
                    foundRelations.AddRange(relationsByType.Values);
                }
            }

            if (_relationsByProvider.ContainsKey(element.Id))
            {
                foreach (Dictionary<string, DsmRelation> relationsByType in _relationsByProvider[element.Id].Values)
                {
                    foundRelations.AddRange(relationsByType.Values);
                }
            }

            foreach (IDsmElement child in element.Children)
            {
                FindRelations(child, ref foundRelations);
            }
        }

        private void OnBeforeElementChangeParent(object sender, IDsmElement element)
        {
            foreach(IDsmRelation relation in FindRelations(element))
            {
                RemoveWeights(relation);
            }
        }

        private void OnAfterElementChangeParent(object sender, IDsmElement element)
        {
            foreach (IDsmRelation relation in FindRelations(element))
            {
                AddWeights(relation);
            }
        }


        private void AddWeights(IDsmRelation relation)
        {
            IDsmElement currentConsumer = relation.Consumer;
            while (currentConsumer != null)
            {
                IDsmElement currentProvider = relation.Provider;
                while (currentProvider != null)
                {
                    AddWeight(currentConsumer.Id, currentProvider.Id, relation.Weight);
                    currentProvider = currentProvider.Parent;
                }
                currentConsumer = currentConsumer.Parent;
            }
        }

        private void RemoveWeights(IDsmRelation relation)
        {
            int consumerId = relation.Consumer.Id;
            int providerId = relation.Provider.Id;

            IDsmElement currentConsumer = relation.Consumer;
            while (currentConsumer != null)
            {
                IDsmElement currentProvider = relation.Provider;
                while (currentProvider != null)
                {
                    RemoveWeight(currentConsumer.Id, currentProvider.Id, relation.Weight);
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
            var weights = _weights[consumerId];
            if (weights.ContainsKey(providerId))
            {
                oldWeight = weights[providerId];
            }
            weights[providerId] = oldWeight + weight;
        }

        private void RemoveWeight(int consumerId, int providerId, int weight)
        {
            if (_weights.ContainsKey(consumerId))
            {
                var weights = _weights[consumerId];
                if (weights.ContainsKey(providerId))
                {
                    int currentWeight = weights[providerId];

                    if (currentWeight >= weight)
                    {
                        weights[providerId] -= weight;
                    }
                    else
                    {
                        //Logger.LogError($"Weight defined between consumerId={consumerId} and providerId={providerId} too low currentWeight={currentWeight} weight={weight}");
                    }

                    if (weights[providerId] == 0)
                    {
                        weights.Remove(providerId);

                        if (weights.Count == 0)
                        {
                            _weights.Remove(consumerId);
                        }
                    }
                }
                else
                {
                    //Logger.LogError($"No weight defined between consumerId={consumerId} and providerId={providerId}");
                }
            }
        }

        private void CountCycles(IDsmElement element, CycleType cycleType, ref int cycleCount)
        {
            foreach (IDsmElement consumer in element.Children)
            {
                foreach (IDsmElement provider in element.Children)
                {
                    if (IsCyclicDependency(consumer.Id, provider.Id) == cycleType)
                    {
                        cycleCount++;
                    }
                }
            }

            foreach (IDsmElement child in element.Children)
            {
                CountCycles(child, cycleType, ref cycleCount);
            }
        }
    }
}

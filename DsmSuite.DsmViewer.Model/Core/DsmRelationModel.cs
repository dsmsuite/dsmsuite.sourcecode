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
        }

        public void Clear()
        {
            _relationsById.Clear();
            _relationsByProvider.Clear();
            _relationsByConsumer.Clear();
            _deletedRelationsById.Clear();

            _lastRelationId = 0;
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

        public int GetDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            int weight = 0;
            DsmElement element = consumer as DsmElement;
            if (element != null)
            {
                weight = element.Dependencies.GetDerivedDependencyWeight(provider);
            }
            return weight;
        }

        public int GetDirectDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            int weight = 0;
            DsmElement element = consumer as DsmElement;
            if (element != null)
            {
                weight = element.Dependencies.GetDirectDependencyWeight(provider);
            }
            return weight;
        }

        public CycleType IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            if ((GetDirectDependencyWeight(consumer, provider) > 0) &&
                (GetDirectDependencyWeight(provider, consumer) > 0))
            {
                return CycleType.System;
            }
            else if ((GetDependencyWeight(consumer, provider) > 0) &&
                     (GetDependencyWeight(provider, consumer) > 0))
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

        public IDsmRelation FindRelation(IDsmElement consumer, IDsmElement provider, string type)
        {
            IDsmRelation relation = null;
            if (_relationsByConsumer.ContainsKey(consumer.Id) &&
                _relationsByConsumer[consumer.Id].ContainsKey(provider.Id) &&
                _relationsByConsumer[consumer.Id][provider.Id].ContainsKey(type))
            {
                relation = _relationsByConsumer[consumer.Id][provider.Id][type];
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
            HashSet<int> elementIds = GetIdsOfElementAndItsChidren(element);
            foreach (int elementId in elementIds)
            {
                if (_relationsByProvider.ContainsKey(elementId))
                {
                    foreach (Dictionary<string, DsmRelation> relationPerType in _relationsByProvider[elementId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (!elementIds.Contains(relation.Consumer.Id) && !relation.IsDeleted)
                            {
                                relations.Add(relation);
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
            HashSet<int> elementIds = GetIdsOfElementAndItsChidren(element);
            foreach (int elementId in elementIds)
            {
                if (_relationsByConsumer.ContainsKey(elementId))
                {
                    foreach (Dictionary<string, DsmRelation> relationPerType in _relationsByConsumer[elementId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (!elementIds.Contains(relation.Provider.Id) && !relation.IsDeleted)
                            {
                                relations.Add(relation);
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
            HashSet<int> elementIds = GetIdsOfElementAndItsChidren(element);
            foreach (int elementId in elementIds)
            {
                if (_relationsByConsumer.ContainsKey(elementId))
                {
                    foreach (Dictionary<string, DsmRelation> relationPerType in _relationsByConsumer[elementId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (elementIds.Contains(relation.Provider.Id) && !relation.IsDeleted)
                            {
                                relations.Add(relation);
                            }
                        }
                    }
                }
            }
            return relations;
        }

        public IEnumerable<DsmRelation> FindExternalRelations(IDsmElement element)
        {
            List<DsmRelation> relations = new List<DsmRelation>();
            HashSet<int> elementIds = GetIdsOfElementAndItsChidren(element);
            foreach (int elementId in elementIds)
            {
                if (_relationsByConsumer.ContainsKey(elementId))
                {
                    foreach (Dictionary<string, DsmRelation> relationsByType in _relationsByConsumer[elementId].Values)
                    {
                        foreach (DsmRelation relation in relationsByType.Values)
                        {
                            if (!elementIds.Contains(relation.Provider.Id) && !relation.IsDeleted)
                            {
                                relations.Add(relation);
                            }
                        }
                    }
                }

                if (_relationsByProvider.ContainsKey(elementId))
                {
                    foreach (Dictionary<string, DsmRelation> relationsByType in _relationsByProvider[elementId].Values)
                    {
                        foreach (DsmRelation relation in relationsByType.Values)
                        {
                            if (!elementIds.Contains(relation.Consumer.Id) && !relation.IsDeleted)
                            {
                                relations.Add(relation);
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

        private IDictionary<int, DsmElement> GetElementAndItsChidren(IDsmElement element)
        {
            Dictionary<int, DsmElement> elements = new Dictionary<int, DsmElement>();
            GetElementAndItsChidren(element, elements);
            return elements;
        }

        private void GetElementAndItsChidren(IDsmElement element, Dictionary<int, DsmElement> elements)
        {
            if (!element.IsDeleted)
            {
                elements[element.Id] = element as DsmElement;
            }

            foreach (IDsmElement child in element.Children)
            {
                GetElementAndItsChidren(child, elements);
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

            DsmElement element = relation.Consumer as DsmElement;
            element.Dependencies.AddDirectWeight(relation.Provider, relation.Weight);

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

            DsmElement element = relation.Consumer as DsmElement;
            element.Dependencies.RemoveDirectWeight(relation.Provider, relation.Weight);

            RemoveWeights(relation);
        }

        private void OnBeforeElementChangeParent(object sender, IDsmElement element)
        {
            foreach (IDsmRelation relation in FindExternalRelations(element))
            {
                RemoveWeights(relation);
            }
        }

        private void OnAfterElementChangeParent(object sender, IDsmElement element)
        {
            foreach (IDsmRelation relation in FindExternalRelations(element))
            {
                AddWeights(relation);
            }
        }

        private void AddWeights(IDsmRelation relation)
        {
            DsmElement currentConsumer = relation.Consumer as DsmElement;
            while (currentConsumer != null)
            {
                IDsmElement currentProvider = relation.Provider;
                while (currentProvider != null)
                {
                    currentConsumer.Dependencies.AddDerivedWeight(currentProvider, relation.Weight);
                    currentProvider = currentProvider.Parent;
                }
                currentConsumer = currentConsumer.Parent as DsmElement;
            }
        }

        private void RemoveWeights(IDsmRelation relation)
        {
            DsmElement currentConsumer = relation.Consumer as DsmElement;
            while (currentConsumer != null)
            {
                IDsmElement currentProvider = relation.Provider;
                while (currentProvider != null)
                {
                    currentConsumer.Dependencies.RemoveDerivedWeight(currentProvider, relation.Weight);
                    currentProvider = currentProvider.Parent;
                }
                currentConsumer = currentConsumer.Parent as DsmElement; ;
            }
        }

        private void CountCycles(IDsmElement element, CycleType cycleType, ref int cycleCount)
        {
            foreach (IDsmElement consumer in element.Children)
            {
                foreach (IDsmElement provider in element.Children)
                {
                    if (IsCyclicDependency(consumer, provider) == cycleType)
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

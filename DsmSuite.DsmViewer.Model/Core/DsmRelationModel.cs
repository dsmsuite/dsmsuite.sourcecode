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
        private int _lastRelationId;

        public DsmRelationModel(DsmElementModel elementsDataModel)
        {
            _elementsDataModel = elementsDataModel;
            _elementsDataModel.ElementUnregistered += OnElementUnregistered;
            _elementsDataModel.ElementReregistered += OnElementReregistered;

            _relationsById = new Dictionary<int, DsmRelation>();
            _relationsByProvider = new Dictionary<int, Dictionary<int, Dictionary<string, DsmRelation>>>();
            _relationsByConsumer = new Dictionary<int, Dictionary<int, Dictionary<string, DsmRelation>>>();
            _deletedRelationsById = new Dictionary<int, DsmRelation>();
            _lastRelationId = 0;
            _weights = new Dictionary<int, Dictionary<int, int>>();
        }
        
        public void Clear()
        {
            _relationsById.Clear();
            _relationsByProvider.Clear();
            _relationsByConsumer.Clear();
            _deletedRelationsById.Clear();

            _lastRelationId = 0;

            _weights.Clear();
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

        public void EditRelationType(IDsmRelation relation, string type)
        {
            DsmRelation editedRelation = relation as DsmRelation;
            if (editedRelation != null)
            {
                UnregisterRelation(editedRelation);

                editedRelation.Type = type;

                RegisterRelation(editedRelation);
            }
        }

        public void EditRelationWeight(IDsmRelation relation, int weight)
        {
            DsmRelation editedRelation = relation as DsmRelation;
            if (editedRelation != null)
            {
                UnregisterRelation(editedRelation);

                editedRelation.Weight = weight;

                RegisterRelation(editedRelation);
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

        public bool IsCyclicDependency(int consumerId, int providerId)
        {
            return (GetDependencyWeight(consumerId, providerId) > 0) &&
                   (GetDependencyWeight(providerId, consumerId) > 0);
        }

        public IDsmRelation GetRelationById(int id)
        {
            return _relationsById.ContainsKey(id) ? _relationsById[id] : null;
        }

        public IDsmRelation GetDeletedRelationById(int id)
        {
            return _deletedRelationsById.ContainsKey(id) ? _deletedRelationsById[id] : null;
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
                            relations.Add(relation);
                        }
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
                    foreach (Dictionary <string, DsmRelation> relationPerType in _relationsByProvider[providerId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (!providerIds.Contains(relation.ConsumerId))
                            {
                                relations.Add(relation);
                            }
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
                    foreach (Dictionary<string, DsmRelation> relationPerType in _relationsByConsumer[consumerId].Values)
                    {
                        foreach (DsmRelation relation in relationPerType.Values)
                        {
                            if (!consumerIds.Contains(relation.ProviderId))
                            {
                                relations.Add(relation);
                            }
                        }
                    }
                }
            }
            return relations;
        }

        public IEnumerable<IDsmRelation> GetExportedRelations()
        {
            return _relationsById.Values.OrderBy(x => x.Id);
        }

        public int GetExportedRelationCount()
        {
            return _relationsById.Values.Count;
        }

        private void OnElementUnregistered(object sender, IDsmElement element)
        {
            List<DsmRelation> toBeRelationsUnregistered = new List<DsmRelation>();
             
            foreach(DsmRelation relation in _relationsById.Values)
            {
                if ((element.Id == relation.ConsumerId) ||
                    (element.Id == relation.ProviderId))
                {
                    toBeRelationsUnregistered.Add(relation);
                }
            }

            foreach(DsmRelation relation in toBeRelationsUnregistered)
            {
                UnregisterRelation(relation);
            }
        }

        private void OnElementReregistered(object sender, IDsmElement element)
        {
            List<DsmRelation> toBeRelationsReregistered = new List<DsmRelation>();

            foreach (DsmRelation relation in _deletedRelationsById.Values)
            {
                if ((element.Id == relation.ConsumerId) ||
                    (element.Id == relation.ProviderId))
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
            ids.Add(element.Id);

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

            if (!_relationsByProvider.ContainsKey(relation.ProviderId))
            {
                _relationsByProvider[relation.ProviderId] = new Dictionary<int, Dictionary<string, DsmRelation>>();
            }

            if (!_relationsByProvider[relation.ProviderId].ContainsKey(relation.ConsumerId))
            {
                _relationsByProvider[relation.ProviderId][relation.ConsumerId] = new Dictionary<string, DsmRelation>();
            }

            _relationsByProvider[relation.ProviderId][relation.ConsumerId][relation.Type] = relation;

            if (!_relationsByConsumer.ContainsKey(relation.ConsumerId))
            {
                _relationsByConsumer[relation.ConsumerId] = new Dictionary<int, Dictionary<string, DsmRelation>>();
            }

            if (!_relationsByConsumer[relation.ConsumerId].ContainsKey(relation.ProviderId))
            {
                _relationsByConsumer[relation.ConsumerId][relation.ProviderId] = new Dictionary<string, DsmRelation>();
            }

            _relationsByConsumer[relation.ConsumerId][relation.ProviderId][relation.Type] = relation;

            UpdateWeights(relation, AddWeight);
        }

        private void UnregisterRelation(DsmRelation relation)
        {
            relation.IsDeleted = true;
            _relationsById.Remove(relation.Id);

            _deletedRelationsById[relation.Id] = relation;

            // TODO: Cleanup collection
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

using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Dependencies
{
    /// <summary>
    /// Manages all element and relation data.
    /// </summary>
    public class DependencyModel
    {
        /// <summary>
        /// Elements by id. The id is assigned once based and is never changed when editing the model.
        /// It can there for be used to track changes to the model.
        /// </summary>
        private readonly Dictionary<int /*id*/, DsmElement> _elementsById = new Dictionary<int, DsmElement>();

        /// <summary>
        /// Element relations sorted by provider first and consumer second.
        /// </summary>
        private readonly Dictionary<int /*providerId*/, Dictionary<int /*consumerId*/, DsmRelation>> _relationsByProvider = new Dictionary<int, Dictionary<int, DsmRelation>>();

        /// <summary>
        /// Element relations sorted by consumer first and provider second.
        /// </summary>
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, DsmRelation>> _relationsByConsumer = new Dictionary<int, Dictionary<int, DsmRelation>>();

        /// <summary>
        /// Dependency weight between consumer and provider. This weight is calculated based on the weights on the selected elements and its children.
        /// </summary>
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, int /*weight*/>> _weights = new Dictionary<int, Dictionary<int, int>>();

        /// <summary>
        /// The root elements
        /// </summary>
        private readonly IList<IDsmElement> _rootElements;

        private int _systemCycleCount = 0;
        private int _directRelationCount = 0;
        private int _relationCount = 0;
        private int _hierarchicalCycalityCount = 0;

        /// <summary>
        /// Delegate used to add or subtract dependency weights.
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="providerId"></param>
        /// <param name="weight"></param>
        private delegate void ModifyWeight(int consumerId, int providerId, int weight);

        /// <summary>
        /// The root elements hierarchy.
        /// </summary>
        public IList<IDsmElement> RootElements => _rootElements;

        public DependencyModel()
        {
            _rootElements = new List<IDsmElement>();
        }

        public void Clear()
        {
            _elementsById.Clear();
            _relationsByProvider.Clear();
            _relationsByConsumer.Clear();
            _weights.Clear();
            _rootElements.Clear();
        }

        /// <summary>
        /// Create element in selected parent. The element id will be assigned based on the fullname
        /// of the element.
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <param name="type">The type of element</param>
        /// <param name="parentId">The element id of the parent</param>
        /// <returns></returns>
        public IDsmElement CreateElement(string name, string type, int? parentId)
        {
            string fullname = name;
            if (parentId.HasValue)
            {
                if (_elementsById.ContainsKey(parentId.Value))
                {
                    HierarchicalName elementName = new HierarchicalName(_elementsById[parentId.Value].Fullname);
                    elementName.Add(name);
                    fullname = elementName.FullName;
                }
            }

            int elementId = fullname.GetHashCode();

            return GetElementById(elementId) ?? AddElement(elementId, name, type, 0, false, parentId);
        }

        /// <summary>
        /// Adds element to selected parent.
        /// </summary>
        /// <param name="id">The element id of the element</param>
        /// <param name="name">The name of the element</param>
        /// <param name="type">The type of element</param>
        /// <param name="order">The order of the element in the hierarchy</param>
        /// <param name="expanded">The element is expanded in the viewer</param>
        /// <param name="parentId">The element id of the parent</param>
        /// <returns></returns>

        public IDsmElement AddElement(int id, string name, string type, int order, bool expanded, int? parentId)
        {
            DsmElement element = null;

            if (parentId.HasValue)
            {
                if (_elementsById.ContainsKey(parentId.Value))
                {
                    element = new DsmElement(id, name, type) { Order = order, IsExpanded = expanded };

                    if (_elementsById.ContainsKey(parentId.Value))
                    {
                        _elementsById[parentId.Value].AddChild(element);
                        RegisterElement(element);
                    }
                    else
                    {
                        Logger.LogError($"Parent not found id={id}");
                    }

                }
            }
            else
            {
                element = new DsmElement(id, name, type) { Order = order, IsExpanded = expanded };
                _rootElements.Add(element);
                RegisterElement(element);
            }

            return element;
        }

        /// <summary>
        /// Remove the element and its children from the model.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveElement(int id)
        {
            if (_elementsById.ContainsKey(id))
            {
                DsmElement element = _elementsById[id];
                UnregisterElement(element);

                foreach (IDsmElement child in element.Children)
                {
                    RemoveElement(child.Id);
                }
            }
        }

        public void RestoreElement(int id)
        {
            throw new NotImplementedException();
        }

        public bool Swap(IDsmElement element1, IDsmElement element2)
        {
            bool swapped = false;

            if (element1.Parent == element2.Parent)
            {
                DsmElement parent = element1.Parent as DsmElement;
                if (parent != null)
                {
                    if (parent.Swap(element1, element2))
                    {
                        swapped = true;
                    }
                }
            }

            AssignElementOrder();

            return swapped;
        }

        /// <summary>
        /// Get element by its id. This id is assigned once and can never change.
        /// </summary>
        /// <param name="id">The element id to be looked for</param>
        /// <returns>The found element or null</returns>
        public DsmElement GetElementById(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        /// <summary>
        /// Get element by its fully qualified name. This name can change when editing the model.
        /// </summary>
        /// <param name="fullname">The fully qualified element name to be looked for</param>
        /// <returns></returns>
        public DsmElement GetElementByFullname(string fullname)
        {
            IEnumerable<DsmElement> elementWithName = from element in _elementsById.Values
                                                      where element.Fullname == fullname
                                                      select element;

            return elementWithName.FirstOrDefault();
        }

        /// <summary>
        /// Get element by its fully qualified name. This name can change when editing the model.
        /// </summary>
        /// <param name="fullname">The fully qualified element name to be looked for</param>
        /// <returns></returns>
        public IEnumerable<IDsmElement> GetElementsWithFullnameContainingText(string text)
        {
            return from element in _elementsById.Values
                   where element.Fullname.Contains(text)
                   select element;
        }

        /// <summary>
        /// The number of elements in the model (minus root element)
        /// </summary>
        public int ElementCount => _elementsById.Count;


        /// <summary>
        /// Add a relation  between two elements.
        /// </summary>
        /// <param name="consumerId">The consumer</param>
        /// <param name="providerId">The provider</param>
        /// <param name="type">The type of relation</param>
        /// <param name="weight">The weight or strength of the relation</param>
        public void AddRelation(int consumerId, int providerId, string type, int weight)
        {
            if (consumerId != providerId)
            {
                DsmRelation relation = new DsmRelation(consumerId, providerId, type, weight);
                RegisterRelation(relation);
            }
        }

        public void RemoveRelation(int consumerId, int providerId, string type, int weight)
        {
            throw new NotImplementedException();
        }

        public void UnremoveRelation(int consumerId, int providerId, string type, int weight)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove the relation from the model.
        /// </summary>
        /// <param name="relation">The relation to be removed</param>
        public void RemoveRelation(IDsmRelation relation)
        {
            UnregisterRelation(relation);
        }

        /// <summary>
        /// Get all relations in the model.
        /// </summary>
        public ICollection<DsmRelation> Relations
        {
            get
            {
                List<DsmRelation> relations = new List<DsmRelation>();
                foreach (Dictionary<int, DsmRelation> value in _relationsByProvider.Values)
                {
                    relations.AddRange(value.Values);
                }
                return relations;
            }
        }

        /// <summary>
        /// The number of relations in the model.
        /// </summary>
        public int RelationCount => Relations.Count;

        public int RelationDensity => 0;

        public IList<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            IList<IDsmRelation> relations = new List<IDsmRelation>();
            List<int> consumerIds = GetIdsOfElementAndItsChidren(consumer);
            List<int> providerIds = GetIdsOfElementAndItsChidren(provider);
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

        public IList<IDsmRelation> FindElementConsumerRelations(IDsmElement element)
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            List<int> providerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int providerId in providerIds)
            {
                if (_relationsByProvider.ContainsKey(providerId))
                {
                    relations.AddRange(_relationsByProvider[providerId].Values);
                }
            }
            return relations;
        }

        public IList<IDsmRelation> FindElementProviderRelations(IDsmElement element)
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            List<int> consumerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int consumerId in consumerIds)
            {
                if (_relationsByConsumer.ContainsKey(consumerId))
                {
                    relations.AddRange(_relationsByConsumer[consumerId].Values);
                }
            }
            return relations;
        }

        public IList<IDsmElement> FindElementProviders(IDsmElement element)
        {
            HashSet<IDsmElement> providers = new HashSet<IDsmElement>();
            foreach (IDsmRelation relation in FindElementProviderRelations(element))
            {
                IDsmElement provider = GetElementById(relation.ProviderId);
                if (provider != null)
                {
                    providers.Add(provider);
                }
            }
            return providers.ToList();
        }

        public IList<IDsmElement> FindElementConsumers(IDsmElement element)
        {
            HashSet<IDsmElement> consumers = new HashSet<IDsmElement>();
            foreach (IDsmRelation relation in FindElementConsumerRelations(element))
            {
                IDsmElement consumer = GetElementById(relation.ConsumerId);
                if (consumer != null)
                {
                    consumers.Add(consumer);
                }
            }
            return consumers.ToList();
        }

        public void AssignElementOrder()
        {
            int order = 1;
            foreach (IDsmElement root in _rootElements)
            {
                DsmElement rootElement = root as DsmElement;
                if (rootElement != null)
                {
                    AssignElementOrder(rootElement, ref order);
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

        public int SystemCycalityCount => _systemCycleCount;

        public double SystemCycalityPercentage
        {
            get
            {
                if (_directRelationCount > 0)
                {
                    return (_systemCycleCount * 100.0) / _directRelationCount;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        public int HierarchicalCycalityCount => _hierarchicalCycalityCount;

        public double HierarchicalCycalityPercentage
        {
            get
            {
                if (_relationCount > 0)
                {
                    return (_hierarchicalCycalityCount * 100.0) / _relationCount;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        private void AssignElementOrder(DsmElement element, ref int order)
        {
            element.Order = order;
            order++;

            foreach (IDsmElement child in element.Children)
            {
                DsmElement childElement = child as DsmElement;
                if (childElement != null)
                {
                    AssignElementOrder(childElement, ref order);
                }
            }
        }

        private void UpdateWeights(IDsmRelation relation, ModifyWeight modifyWeight)
        {
            int consumerId = relation.ConsumerId;
            int providerId = relation.ProviderId;

            if (_elementsById.ContainsKey(consumerId))
            {
                IDsmElement currentConsumer = _elementsById[consumerId];
                while (currentConsumer != null)
                {
                    IDsmElement currentProvider = _elementsById[providerId];
                    while (currentProvider != null)
                    {
                        modifyWeight(currentConsumer.Id, currentProvider.Id, relation.Weight);
                        currentProvider = currentProvider.Parent;
                    }
                    currentConsumer = currentConsumer.Parent;
                }
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

        private List<int> GetIdsOfElementAndItsChidren(IDsmElement element)
        {
            List<int> ids = new List<int>();
            GetIdsOfElementAndItsChidren(element, ids);
            return ids;
        }

        private void GetIdsOfElementAndItsChidren(IDsmElement element, List<int> ids)
        {
            ids.Add(element.Id);

            foreach (IDsmElement child in element.Children)
            {
                GetIdsOfElementAndItsChidren(child, ids);
            }
        }

        private void RegisterElement(DsmElement element)
        {
            _elementsById[element.Id] = element;
        }

        private void UnregisterElement(IDsmElement element)
        {
            UnregisterProviderRelations(element);
            UnregisterConsumerRelations(element);

            _elementsById.Remove(element.Id);
        }

        private void RegisterRelation(DsmRelation relation)
        {
            if (!_relationsByProvider.ContainsKey(relation.ProviderId))
            {
                _relationsByProvider[relation.ProviderId] = new Dictionary<int, DsmRelation>();
            }
            _relationsByProvider[relation.ProviderId][relation.ConsumerId] = relation;

            if (!_relationsByConsumer.ContainsKey(relation.ConsumerId))
            {
                _relationsByConsumer[relation.ConsumerId] = new Dictionary<int, DsmRelation>();
            }
            _relationsByConsumer[relation.ConsumerId][relation.ProviderId] = relation;

            UpdateWeights(relation, AddWeight);
        }

        private void UnregisterRelation(IDsmRelation relation)
        {
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

        private void UnregisterConsumerRelations(IDsmElement element)
        {
            if (_relationsByConsumer.ContainsKey(element.Id))
            {
                _relationsByConsumer.Remove(element.Id);
            }

            foreach (Dictionary<int, DsmRelation> consumerRelations in _relationsByConsumer.Values)
            {
                if (consumerRelations.ContainsKey(element.Id))
                {
                    consumerRelations.Remove(element.Id);
                }
            }
        }

        private void UnregisterProviderRelations(IDsmElement element)
        {
            if (_relationsByProvider.ContainsKey(element.Id))
            {
                _relationsByProvider.Remove(element.Id);
            }

            foreach (Dictionary<int, DsmRelation> providerRelations in _relationsByProvider.Values)
            {
                if (providerRelations.ContainsKey(element.Id))
                {
                    providerRelations.Remove(element.Id);
                }
            }
        }
    }
}

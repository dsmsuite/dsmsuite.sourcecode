using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Util;

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
        private readonly Dictionary<int /*id*/, Element> _elementsById = new Dictionary<int, Element>();

        /// <summary>
        /// Element relations sorted by provider first and consumer second.
        /// </summary>
        private readonly Dictionary<int /*providerId*/, Dictionary<int /*consumerId*/, Relation>> _relationsByProvider = new Dictionary<int, Dictionary<int, Relation>>();

        /// <summary>
        /// Element relations sorted by consumer first and provider second.
        /// </summary>
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, Relation>> _relationsByConsumer = new Dictionary<int, Dictionary<int, Relation>>();

        /// <summary>
        /// Dependency weight between consumer and provider. This weight is calculated based on the weights on the selected elements and its children.
        /// </summary>
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, int /*weight*/>> _weights = new Dictionary<int, Dictionary<int, int>>();

        /// <summary>
        /// The root elements
        /// </summary>
        private readonly IList<IElement> _rootElements;

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
        public IList<IElement> RootElements => _rootElements;

        public DependencyModel()
        {
            _rootElements = new List<IElement>();
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
        public IElement CreateElement(string name, string type, int? parentId)
        {
            string fullname = name;
            if (parentId.HasValue)
            {
                if (_elementsById.ContainsKey(parentId.Value))
                {
                    HierarchicalName elementName =  new HierarchicalName(_elementsById[parentId.Value].Fullname);
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

        public IElement AddElement(int id, string name, string type, int order, bool expanded, int? parentId)
        {
            Element element = null;

            if (parentId.HasValue)
            {
                if (_elementsById.ContainsKey(parentId.Value))
                {
                    element = new Element(id, name, type) {Order = order, IsExpanded = expanded};

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
                element = new Element(id, name, type) { Order = order, IsExpanded = expanded };
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
                Element element = _elementsById[id];
                UnregisterElement(element);

                foreach (IElement child in element.Children)
                {
                    RemoveElement(child.Id);
                }
            }
        }

        public void RestoreElement(int id)
        {
            throw new NotImplementedException();
        }

        public bool Swap(IElement element1, IElement element2)
        {
            bool swapped = false;

            if (element1.Parent == element2.Parent)
            {
                Element parent = element1.Parent as Element;
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
        public Element GetElementById(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        /// <summary>
        /// Get element by its fully qualified name. This name can change when editing the model.
        /// </summary>
        /// <param name="fullname">The fully qualified element name to be looked for</param>
        /// <returns></returns>
        public Element GetElementByFullname(string fullname)
        {
            IEnumerable<Element> elementWithName = from element in _elementsById.Values
                                                   where element.Fullname == fullname
                                                   select element;

            return elementWithName.FirstOrDefault();
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
                Relation relation = new Relation(consumerId, providerId, type, weight);
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
        public void RemoveRelation(Relation relation)
        {
            UnregisterRelation(relation);
        }

        /// <summary>
        /// Get all relations in the model.
        /// </summary>
        public ICollection<Relation> Relations
        {
            get
            {
                List<Relation> relations = new List<Relation>();
                foreach (Dictionary<int, Relation> value in _relationsByProvider.Values)
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

        public IList<Relation> FindRelations(IElement consumer, IElement provider)
        {
            IList<Relation> relations = new List<Relation>();
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

        public IList<Relation> FindElementConsumerRelations(IElement element)
        {
            List<Relation> relations = new List<Relation>();
            List<int> providerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int providerId in providerIds)
            {
                if (_relationsByProvider.ContainsKey(providerId))
                {
                    relations.AddRange(_relationsByProvider[providerId].Values.ToList());
                }
            }
            return relations;
        }

        public IList<Relation> FindElementProviderRelations(IElement element)
        {
            List<Relation> relations = new List<Relation>();
            List<int> consumerIds = GetIdsOfElementAndItsChidren(element);
            foreach (int consumerId in consumerIds)
            {
                if (_relationsByConsumer.ContainsKey(consumerId))
                {
                    relations.AddRange(_relationsByConsumer[consumerId].Values.ToList());
                }
            }
            return relations;
        }

        public IList<Element> FindElementProviders(IElement element)
        {
            HashSet<Element> providers = new HashSet<Element>();
            foreach (Relation relation in FindElementProviderRelations(element))
            {
                Element provider = GetElementById(relation.ProviderId);
                if (provider != null)
                {
                    providers.Add(provider);
                }
            }
            return providers.ToList();
        }

        public IList<Element> FindElementConsumers(IElement element)
        {
            HashSet<Element> consumers = new HashSet<Element>();
            foreach (Relation relation in FindElementConsumerRelations(element))
            {
                Element consumer = GetElementById(relation.ConsumerId);
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
            foreach (IElement root in _rootElements)
            {
                Element rootElement = root as Element;
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

        private void AssignElementOrder(Element element, ref int order)
        {
            element.Order = order;
            order++;

            foreach (IElement child in element.Children)
            {
                Element childElement = child as Element;
                if (childElement != null)
                {
                    AssignElementOrder(childElement, ref order);
                }
            }
        }

        private void UpdateWeights(Relation relation, ModifyWeight modifyWeight)
        {
            int consumerId = relation.ConsumerId;
            int providerId = relation.ProviderId;

            if (_elementsById.ContainsKey(consumerId))
            {
                IElement currentConsumer = _elementsById[consumerId];
                while (currentConsumer != null)
                {
                    IElement currentProvider = _elementsById[providerId];
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

        private List<int> GetIdsOfElementAndItsChidren(IElement element)
        {
            List<int> ids = new List<int>();
            GetIdsOfElementAndItsChidren(element, ids);
            return ids;
        }

        private void GetIdsOfElementAndItsChidren(IElement element, List<int> ids)
        {
            ids.Add(element.Id);

            foreach (IElement child in element.Children)
            {
                GetIdsOfElementAndItsChidren(child, ids);
            }
        }

        private void RegisterElement(Element element)
        {
            _elementsById[element.Id] = element;
        }

        private void UnregisterElement(IElement element)
        {
            UnregisterProviderRelations(element);
            UnregisterConsumerRelations(element);

            _elementsById.Remove(element.Id);
        }

        private void RegisterRelation(Relation relation)
        {
            if (!_relationsByProvider.ContainsKey(relation.ProviderId))
            {
                _relationsByProvider[relation.ProviderId] = new Dictionary<int, Relation>();
            }
            _relationsByProvider[relation.ProviderId][relation.ConsumerId] = relation;

            if (!_relationsByConsumer.ContainsKey(relation.ConsumerId))
            {
                _relationsByConsumer[relation.ConsumerId] = new Dictionary<int, Relation>();
            }
            _relationsByConsumer[relation.ConsumerId][relation.ProviderId] = relation;

            UpdateWeights(relation, AddWeight);
        }

        private void UnregisterRelation(Relation relation)
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

        private void UnregisterConsumerRelations(IElement element)
        {
            if (_relationsByConsumer.ContainsKey(element.Id))
            {
                _relationsByConsumer.Remove(element.Id);
            }

            foreach (Dictionary<int, Relation> consumerRelations in _relationsByConsumer.Values)
            {
                if (consumerRelations.ContainsKey(element.Id))
                {
                    consumerRelations.Remove(element.Id);
                }
            }
        }

        private void UnregisterProviderRelations(IElement element)
        {
            if (_relationsByProvider.ContainsKey(element.Id))
            {
                _relationsByProvider.Remove(element.Id);
            }

            foreach (Dictionary<int, Relation> providerRelations in _relationsByProvider.Values)
            {
                if (providerRelations.ContainsKey(element.Id))
                {
                    providerRelations.Remove(element.Id);
                }
            }
        }
    }
}

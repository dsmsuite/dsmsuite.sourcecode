using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmModel : IDsmModel, IDsmModelFileCallback
    {
        private readonly string _processStep;
        private bool _isModified;

        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<IDsmMetaDataItem>> _metaDataGroups;

        private readonly Dictionary<int /*id*/, DsmElement> _elementsById;

        private readonly Dictionary<int /*providerId*/, Dictionary<int /*consumerId*/, DsmRelation>> _relationsByProvider;
        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, DsmRelation>> _relationsByConsumer;

        private readonly Dictionary<int /*consumerId*/, Dictionary<int /*providerId*/, int /*weight*/>> _weights;

        private readonly IList<IDsmElement> _rootElements;

        public event EventHandler<bool> Modified;

        public DsmModel(string processStep, Assembly executingAssembly)
        {
            _processStep = processStep;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<IDsmMetaDataItem>>();

            _elementsById = new Dictionary<int, DsmElement>();

            _relationsByProvider = new Dictionary<int, Dictionary<int, DsmRelation>>();
            _relationsByConsumer = new Dictionary<int, Dictionary<int, DsmRelation>>();

            _weights = new Dictionary<int, Dictionary<int, int>>();

            _rootElements = new List<IDsmElement>();

            AddMetaData("Executable", SystemInfo.GetExecutableInfo(executingAssembly));
        }

        public void LoadModel(string dsmFilename, IProgress<DsmProgressInfo> progress)
        {
            Clear();
            DsmModelFile dsmModelFile = new DsmModelFile(dsmFilename, this);
            dsmModelFile.Load(progress);
            IsCompressed = dsmModelFile.IsCompressedFile();
            ModelFilename = dsmFilename;
        }

        public void SaveModel(string dsmFilename, bool compressFile, IProgress<DsmProgressInfo> progress)
        {
            if (_processStep != null)
            {
                AddMetaData("Total elements found", $"{ElementCount}");
            }

            DsmModelFile dsmModelFile = new DsmModelFile(dsmFilename, this);
            dsmModelFile.Save(compressFile, progress);
            IsModified = false;
            ModelFilename = dsmFilename;
        }

        public string ModelFilename { get; private set; }

        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            private set
            {
                _isModified = value;
                Modified?.Invoke(this, _isModified);
            }
        }

        public bool IsCompressed { get; private set; }

        public void Clear()
        {
            _metaDataGroupNames.Clear();
            _metaDataGroups.Clear();

            _elementsById.Clear();

            _relationsByProvider.Clear();
            _relationsByConsumer.Clear();

            _weights.Clear();

            _rootElements.Clear();
        }

        public IDsmMetaDataItem AddMetaData(string name, string value)
        {
            return AddMetaData(_processStep, name, value);
        }

        public IDsmMetaDataItem AddMetaData(string group, string name, string value)
        {
            Logger.LogUserMessage($"Metadata: processStep={@group} name={name} value={value}");

            DsmMetaDataItem metaDataItem = new DsmMetaDataItem(name, value);
            GetMetaDataGroupItemList(@group).Add(metaDataItem);
            return metaDataItem;
        }

        public IDsmMetaDataItem ImportMetaDataItem(string groupName, string name, string value)
        {
            Logger.LogUserMessage($"Metadata: groupName={groupName} name={name} value={value}");

            DsmMetaDataItem metaDataItem = new DsmMetaDataItem(name, value);
            GetMetaDataGroupItemList(groupName).Add(metaDataItem);
            return metaDataItem;
        }

        public IList<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IList<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return GetMetaDataGroupItemList(groupName);
        }

        public IList<IDsmElement> RootElements => _rootElements;

        public IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parentId)
        {
            return AddElement(id, name, type, order, expanded, parentId);
        }

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

        public IList<IDsmElement> GetRootElements()
        {
            return _rootElements;
        }

        public int GetElementCount()
        {
            return _elementsById.Count;
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

        public int ElementCount => _elementsById.Count;

        public IDsmElement GetElementById(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        public IDsmElement GetElementByFullname(string fullname)
        {
            IEnumerable<DsmElement> elementWithName = from element in _elementsById.Values
                                                      where element.Fullname == fullname
                                                      select element;

            return elementWithName.FirstOrDefault();
        }

        public IEnumerable<IDsmElement> GetElementsWithFullnameContainingText(string text)
        {
            return from element in _elementsById.Values
                   where element.Fullname.Contains(text)
                   select element;
        }

        public IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            DsmRelation relation = null;
            if (consumerId != providerId)
            {
                relation = new DsmRelation(consumerId, providerId, type, weight);
                RegisterRelation(relation);
            }
            return relation;
        }

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

        public IList<IDsmRelation> FindProviderRelations(IDsmElement element)
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

        public IList<IDsmRelation> FindConsumerRelations(IDsmElement element)
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

        public IList<IDsmRelation> GetRelations()
        {
            List<IDsmRelation> relations = new List<IDsmRelation>();
            foreach (Dictionary<int, DsmRelation> consumerRelations in _relationsByConsumer.Values)
            {
                relations.AddRange(consumerRelations.Values);
            }
            return relations;
        }

        public int GetRelationCount()
        {
            int relationCount = 0;
            foreach (Dictionary<int, DsmRelation> consumerRelations in _relationsByConsumer.Values)
            {
                relationCount += consumerRelations.Count;
            }
            return relationCount;
        }

        public IList<IDsmResolvedRelation> ResolveRelations(IList<IDsmRelation> relations)
        {
            List<IDsmResolvedRelation> resolvedRelations = new List<IDsmResolvedRelation>();
            foreach (IDsmRelation relation in relations)
            {
                resolvedRelations.Add(new DsmResolvedRelation(_elementsById, relation));
            }
            return resolvedRelations;
        }

        public IList<IDsmElement> FindProviders(IDsmElement element)
        {
            HashSet<IDsmElement> providers = new HashSet<IDsmElement>();
            foreach (IDsmRelation relation in FindConsumerRelations(element))
            {
                IDsmElement provider = GetElementById(relation.ProviderId);
                if (provider != null)
                {
                    providers.Add(provider);
                }
            }
            return providers.ToList();

        }

        public IList<IDsmElement> FindConsumers(IDsmElement element)
        {
            HashSet<IDsmElement> consumers = new HashSet<IDsmElement>();
            foreach (IDsmRelation relation in FindProviderRelations(element))
            {
                IDsmElement consumer = GetElementById(relation.ConsumerId);
                if (consumer != null)
                {
                    consumers.Add(consumer);
                }
            }
            return consumers.ToList();
        }

        public void ReorderChildren(IDsmElement element, Vector permutationVector)
        {
            DsmElement parent = element as DsmElement;
            if (parent != null)
            {
                List<IDsmElement> clonedChildren = new List<IDsmElement>(parent.Children);

                foreach (IDsmElement child in clonedChildren)
                {
                    parent.RemoveChild(child);
                }

                for (int i = 0; i < permutationVector.Size; i++)
                {
                    parent.AddChild(clonedChildren[permutationVector.Get(i)]);
                }
            }
            AssignElementOrder();
            IsModified = true;
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

        public IDsmElement NextSibling(IDsmElement element)
        {
            IDsmElement next = null;
            if (element != null)
            {
                next = element.NextSibling;
            }
            return next;
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            IDsmElement previous = null;
            if (element != null)
            {
                previous = element.PreviousSibling;
            }
            return previous;
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

        private IDsmElement AddElement(int id, string name, string type, int order, bool expanded, int? parentId)
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

        /// <summary>
        /// Delegate used to add or subtract dependency weights.
        /// </summary>
        /// <param name="consumerId"></param>
        /// <param name="providerId"></param>
        /// <param name="weight"></param>
        private delegate void ModifyWeight(int consumerId, int providerId, int weight);

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

        private IList<IDsmMetaDataItem> GetMetaDataGroupItemList(string groupName)
        {
            if (!_metaDataGroups.ContainsKey(groupName))
            {
                _metaDataGroupNames.Add(groupName);
                _metaDataGroups[groupName] = new List<IDsmMetaDataItem>();
            }

            return _metaDataGroups[groupName];
        }
    }
}

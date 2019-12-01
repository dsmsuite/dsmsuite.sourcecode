using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Util;
using DsmSuite.Analyzer.Util;

namespace DsmSuite.Analyzer.Model.Core
{
    /// <summary>
    /// The data model maintains data item and allows persisting them to a file.
    /// </summary>
    public class DataModel : IDataModel, IDsiModelFileCallback
    {
        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<IMetaDataItem>> _metaDataGroups;
        private readonly string _processStep;

        private readonly Dictionary<string, IElement> _elementsByName;
        private readonly Dictionary<int, IElement> _elementsById;
        private readonly Dictionary<string, int> _elementTypeCount;

        private readonly Dictionary<int, List<IRelation>> _relationsByConsumerId;
        private readonly Dictionary<string, int> _relationTypeCount;
        private int _relationCount;

        public DataModel(string processStep, Assembly executingAssembly)
        {
            _processStep = processStep;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<IMetaDataItem>>();

            _elementsByName = new Dictionary<string, IElement>();
            _elementsById = new Dictionary<int, IElement>();
            _elementTypeCount = new Dictionary<string, int>();

            _relationsByConsumerId = new Dictionary<int, List<IRelation>>();
            _relationTypeCount = new Dictionary<string, int>();

            AddMetaData("Executable", SystemInfo.GetExecutableInfo(executingAssembly));
        }

        public void Load(string dsiFilename)
        {
            AnalyzerLogger.LogDataModelAction("Load data model file=" + dsiFilename);

            DsiModelFile modelFile = new DsiModelFile(dsiFilename, this);
            modelFile.Load(null);
        }

        public void Save(string dsiFilename, bool compressFile)
        {
            AnalyzerLogger.LogDataModelAction("Save data model file=" + dsiFilename);

            foreach (string type in GetElementTypes())
            {
                AddMetaData($"- '{type}' elements found", $"{GetElementTypeCount(type)}");
            }
            AddMetaData("Total elements found", $"{TotalElementCount}");

            foreach (string type in GetRelationTypes())
            {
                AddMetaData($"- '{type}' relations found", $"{GetRelationTypeCount(type)}");
            }
            AddMetaData("Total relations found", $"{TotalRelationCount}");
            AddMetaData("Total relations resolved", $"{ResolvedRelationCount} (confidence={ResolvedRelationPercentage:0.000} %)");

            DsiModelFile modelFile = new DsiModelFile(dsiFilename, this);
            modelFile.Save(compressFile, null);
        }

        public void AddMetaData(string itemName, string itemValue)
        {
            Logger.LogUserMessage($"Metadata: processStep={_processStep} name={itemName} value={itemValue}");

            GetMetaDataGroupItemList(_processStep).Add(new MetaDataItem(itemName, itemValue));
        }

        public void ImportMetaDataItem(string groupName, IMetaDataItem metaDataItem)
        {
            Logger.LogUserMessage($"Metadata: groupName={groupName} name={metaDataItem.Name} value={metaDataItem.Value}");

            GetMetaDataGroupItemList(groupName).Add(metaDataItem);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return GetMetaDataGroupItemList(groupName);
        }
        
        public void ImportElement(IElement element)
        {
            _elementsByName[element.Name] = element;
            _elementsById[element.Id] = element;
            IncrementElementTypeCount(element.Type);
        }
        
        public IElement AddElement(string name, string type, string source)
        {
            AnalyzerLogger.LogDataModelAction("Add element to data model name=" + name + " type=" + type + " source=" + source);

            string key = name.ToLower();
            if (!_elementsByName.ContainsKey(key))
            {
                IncrementElementTypeCount(type);
                int id = _elementsByName.Count;
                Element element = new Element(id, name, type, source);
                _elementsByName[key] = element;
                _elementsById[id] = element;
                return element;
            }
            else
            {
                return null;
            }
        }
        
        public void RemoveElement(IElement element)
        {
            string key = element.Name.ToLower();
            _elementsByName.Remove(key);
            _elementsById.Remove(element.Id);
        }

        public void RenameElement(IElement element, string newName)
        {
            AnalyzerLogger.LogDataModelAction("Rename element in data model from name=" + element.Name + " to name=" + newName);
            Element e = element as Element;
            if (e != null)
            {
                string oldKey = e.Name.ToLower();
                _elementsByName.Remove(oldKey);
                e.Name = newName;
                string newKey = e.Name.ToLower();
                _elementsByName[newKey] = e;
            }
        }

        public IElement FindElement(string name)
        {
            string key = name.ToLower();
            return _elementsByName.ContainsKey(key) ? _elementsByName[key] : null;
        }

        public IElement FindElement(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        public IEnumerable<IElement> GetElements()
        {
            return _elementsById.Values;
        }

        public ICollection<string> GetElementTypes()
        {
            return _elementTypeCount.Keys;
        }

        public int GetElementTypeCount(string type)
        {
            if (_elementTypeCount.ContainsKey(type))
            {
                return _elementTypeCount[type];
            }
            else
            {
                return 0;
            }
        }
        
        public int TotalElementCount => _elementsByName.Values.Count;
        
        public void ImportRelation(IRelation relation)
        {
            IncrementRelationTypeCount(relation.Type);

            if (!_relationsByConsumerId.ContainsKey(relation.ConsumerId))
            {
                _relationsByConsumerId[relation.ConsumerId] = new List<IRelation>();
            }
            _relationsByConsumerId[relation.ConsumerId].Add(relation);
        }

        public IRelation AddRelation(string consumerName, string providerName, string type, int weight, string context)
        {
            AnalyzerLogger.LogDataModelAction("Add relation " + type + " from consumer=" + consumerName + " to provider=" + providerName + " in " + context);
            _relationCount++;

            IElement consumer = FindElement(consumerName);
            IElement provider = FindElement(providerName);
            IRelation relation = null;

            if (consumer != null && provider != null)
            {
                IncrementRelationTypeCount(type);

                relation = new Relation(consumer.Id, provider.Id, type, weight);
                if (!_relationsByConsumerId.ContainsKey(consumer.Id))
                {
                    _relationsByConsumerId[consumer.Id] = new List<IRelation>();
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
            AnalyzerLogger.LogDataModelAction("Skip relation " + type + " from consumer=" + consumerName + " to provider=" + providerName + " in " + context);

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
        
        public ICollection<IRelation> GetProviderRelations(IElement consumer)
        {
            if (_relationsByConsumerId.ContainsKey(consumer.Id))
            {
                return _relationsByConsumerId[consumer.Id];
            }
            else
            {
                return new List<IRelation>();
            }
        }

        public IEnumerable<IRelation> GetRelations()
        {
            List<IRelation> relations = new List<IRelation>();
            foreach (List<IRelation> consumerRelations in _relationsByConsumerId.Values)
            {
                relations.AddRange(consumerRelations);
            }
            return relations;
        }

        public bool DoesRelationExist(IElement consumer, IElement provider)
        {
            bool doesRelationExist = false;

            if (_relationsByConsumerId.ContainsKey(consumer.Id))
            {
                foreach (IRelation relation in _relationsByConsumerId[consumer.Id])
                {
                    if (relation.ProviderId == provider.Id)
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

                foreach (IElement element in _elementsByName.Values)
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


        /// <summary>
        /// Removed relations from and to element which have bee removed.
        /// For performance reasons it is done once after one or more
        /// element removal actions.
        /// </summary>
        public void Cleanup()
        {
            IElement[] elements = _elementsByName.Values.ToArray();

            foreach (IElement element in elements)
            {
                if (_relationsByConsumerId.ContainsKey(element.Id))
                {
                    IRelation[] relations = _relationsByConsumerId[element.Id].ToArray();

                    foreach (IRelation relation in relations)
                    {
                        if (!_elementsById.ContainsKey(relation.ConsumerId) ||
                            !_elementsById.ContainsKey(relation.ProviderId))
                        {
                            _relationsByConsumerId[element.Id].Remove(relation);
                        }
                    }
                }
            }
        }

        private IList<IMetaDataItem> GetMetaDataGroupItemList(string groupName)
        {
            if (!_metaDataGroups.ContainsKey(groupName))
            {
                _metaDataGroupNames.Add(groupName);
                _metaDataGroups[groupName] = new List<IMetaDataItem>();
            }

            return _metaDataGroups[groupName];
        }

        private void IncrementElementTypeCount(string type)
        {
            if (!_elementTypeCount.ContainsKey(type))
            {
                _elementTypeCount[type] = 0;
            }
            _elementTypeCount[type]++;
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

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
    public class DsiDataModel : IDsiDataModel, IDsiModelFileCallback
    {
        private readonly string _processStep;

        private readonly List<string> _metaDataGroupNames;
        private readonly Dictionary<string, List<IDsiMetaDataItem>> _metaDataGroups;

        private readonly Dictionary<string, IDsiElement> _elementsByName;
        private readonly Dictionary<int, IDsiElement> _elementsById;
        private readonly Dictionary<string, int> _elementTypeCount;

        private readonly Dictionary<int, List<IDsiRelation>> _relationsByConsumerId;
        private readonly Dictionary<string, int> _relationTypeCount;
        private int _relationCount;

        public DsiDataModel(string processStep, Assembly executingAssembly)
        {
            _processStep = processStep;

            _metaDataGroupNames = new List<string>();
            _metaDataGroups = new Dictionary<string, List<IDsiMetaDataItem>>();

            _elementsByName = new Dictionary<string, IDsiElement>();
            _elementsById = new Dictionary<int, IDsiElement>();
            _elementTypeCount = new Dictionary<string, int>();

            _relationsByConsumerId = new Dictionary<int, List<IDsiRelation>>();
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

        public void AddMetaData(string name, string value)
        {
            Logger.LogUserMessage($"Metadata: processStep={_processStep} name={name} value={value}");

            DsiMetaDataItem metaDataItem = new DsiMetaDataItem(name, value);
            GetMetaDataGroupItemList(_processStep).Add(metaDataItem);
        }

        public void ImportMetaDataItem(string groupName, string name, string value)
        {
            Logger.LogUserMessage($"Metadata: groupName={groupName} name={name} value={value}");

            DsiMetaDataItem metaDataItem =new DsiMetaDataItem(name, value);
            GetMetaDataGroupItemList(groupName).Add(metaDataItem);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return GetMetaDataGroupItemList(groupName);
        }
        
        public void ImportElement(int id, string name, string type, string source)
        {
            DsiElement element = new DsiElement(id, name, type, source);
            _elementsByName[element.Name] = element;
            _elementsById[element.Id] = element;
            IncrementElementTypeCount(element.Type);
        }
        
        public IDsiElement AddElement(string name, string type, string source)
        {
            AnalyzerLogger.LogDataModelAction("Add element to data model name=" + name + " type=" + type + " source=" + source);

            string key = name.ToLower();
            if (!_elementsByName.ContainsKey(key))
            {
                IncrementElementTypeCount(type);
                int id = _elementsByName.Count;
                DsiElement element = new DsiElement(id, name, type, source);
                _elementsByName[key] = element;
                _elementsById[id] = element;
                return element;
            }
            else
            {
                return null;
            }
        }
        
        public void RemoveElement(IDsiElement element)
        {
            string key = element.Name.ToLower();
            _elementsByName.Remove(key);
            _elementsById.Remove(element.Id);
        }

        public void RenameElement(IDsiElement element, string newName)
        {
            AnalyzerLogger.LogDataModelAction("Rename element in data model from name=" + element.Name + " to name=" + newName);
            DsiElement e = element as DsiElement;
            if (e != null)
            {
                string oldKey = e.Name.ToLower();
                _elementsByName.Remove(oldKey);
                e.Name = newName;
                string newKey = e.Name.ToLower();
                _elementsByName[newKey] = e;
            }
        }

        public IDsiElement FindElement(string name)
        {
            string key = name.ToLower();
            return _elementsByName.ContainsKey(key) ? _elementsByName[key] : null;
        }

        public IDsiElement FindElement(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        public IEnumerable<IDsiElement> GetElements()
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
        
        public void ImportRelation(int consumerId, int providerId, string type, int weight)
        {
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
            AnalyzerLogger.LogDataModelAction("Add relation " + type + " from consumer=" + consumerName + " to provider=" + providerName + " in " + context);
            _relationCount++;

            IDsiElement consumer = FindElement(consumerName);
            IDsiElement provider = FindElement(providerName);
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
        
        public ICollection<IDsiRelation> GetProviderRelations(IDsiElement consumer)
        {
            if (_relationsByConsumerId.ContainsKey(consumer.Id))
            {
                return _relationsByConsumerId[consumer.Id];
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

        public bool DoesRelationExist(IDsiElement consumer, IDsiElement provider)
        {
            bool doesRelationExist = false;

            if (_relationsByConsumerId.ContainsKey(consumer.Id))
            {
                foreach (IDsiRelation relation in _relationsByConsumerId[consumer.Id])
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

                foreach (IDsiElement element in _elementsByName.Values)
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
            IDsiElement[] elements = _elementsByName.Values.ToArray();

            foreach (IDsiElement element in elements)
            {
                if (_relationsByConsumerId.ContainsKey(element.Id))
                {
                    IDsiRelation[] relations = _relationsByConsumerId[element.Id].ToArray();

                    foreach (IDsiRelation relation in relations)
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

        private IList<IDsiMetaDataItem> GetMetaDataGroupItemList(string groupName)
        {
            if (!_metaDataGroups.ContainsKey(groupName))
            {
                _metaDataGroupNames.Add(groupName);
                _metaDataGroups[groupName] = new List<IDsiMetaDataItem>();
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

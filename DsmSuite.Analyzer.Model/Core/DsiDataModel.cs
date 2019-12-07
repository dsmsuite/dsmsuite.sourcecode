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
            Logger.LogDataModelMessage($"Load data model file={dsiFilename}");

            DsiModelFile modelFile = new DsiModelFile(dsiFilename, this);
            modelFile.Load(null);
        }

        public void Save(string dsiFilename, bool compressFile)
        {
            Logger.LogDataModelMessage($"Save data model file={dsiFilename} compresss={compressFile}");

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
            Logger.LogDataModelMessage($"Add metadata group={_processStep} name={name} value={value}");

            GetMetaDataGroupItemList(_processStep).Add(new DsiMetaDataItem(name, value));
        }

        public void ImportMetaDataItem(string group, string name, string value)
        {
            Logger.LogDataModelMessage($"Import meta data group={group} name={name} value={value}");

            GetMetaDataGroupItemList(group).Add(new DsiMetaDataItem(name, value));
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataGroupNames;
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string group)
        {
            return GetMetaDataGroupItemList(group);
        }
        
        public void ImportElement(int id, string name, string type, string source)
        {
            Logger.LogDataModelMessage($"Import element id={id} name={name} type={type} source={source}");

            DsiElement element = new DsiElement(id, name, type, source);
            _elementsByName[element.Name] = element;
            _elementsById[element.Id] = element;
            IncrementElementTypeCount(element.Type);
        }
        
        public IDsiElement AddElement(string name, string type, string source)
        {
            Logger.LogDataModelMessage($"Add element name={name} type={type} source={source}");

            string key = name.ToLower();
            if (!_elementsByName.ContainsKey(key))
            {
                IncrementElementTypeCount(type);
                int id = _elementsByName.Count + 1;
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
            Logger.LogDataModelMessage($"Remove element id={element.Id} name={element.Name} type={element.Type} source={element.Source}");

            string key = element.Name.ToLower();
            _elementsByName.Remove(key);
            _elementsById.Remove(element.Id);
        }

        public void RenameElement(IDsiElement element, string newName)
        {
            Logger.LogDataModelMessage("Rename element id={element.Id} from {element.Name} to {newName}");

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

        public IDsiElement FindElementById(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        public IDsiElement FindElementByName(string name)
        {
            string key = name.ToLower();
            return _elementsByName.ContainsKey(key) ? _elementsByName[key] : null;
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
            Logger.LogDataModelMessage("Import relation consumerId={consumerId} providerId={providerId} type={type} weight={weight}");

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

            IDsiElement consumer = FindElementByName(consumerName);
            IDsiElement provider = FindElementByName(providerName);
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

        private IList<IDsiMetaDataItem> GetMetaDataGroupItemList(string group)
        {
            if (!_metaDataGroups.ContainsKey(group))
            {
                _metaDataGroupNames.Add(group);
                _metaDataGroups[group] = new List<IDsiMetaDataItem>();
            }

            return _metaDataGroups[group];
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

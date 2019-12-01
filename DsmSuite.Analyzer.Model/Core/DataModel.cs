using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using AnalyzerLogger = DsmSuite.Analyzer.Util.AnalyzerLogger;

namespace DsmSuite.Analyzer.Model.Core
{
    /// <summary>
    /// The data model maintains data item and allows persisting them to a file.
    /// </summary>
    public class DataModel : IDataModel
    {
        private readonly List<KeyValuePair<string, List<KeyValuePair<string, string>>>> _metaData;
        private readonly List<KeyValuePair<string, string>> _processStepMetaData;
        private readonly string _processStep;
        private readonly Dictionary<string, IElement> _elementsByCaseInsensitiveName;
        private readonly Dictionary<int, IElement> _elementsById;
        private readonly Dictionary<int, List<IRelation>> _relations;
        private int _relationCount;
        private readonly Dictionary<string, int> _elementTypeCount;
        private readonly Dictionary<string, int> _relationTypeCount;

        public DataModel(string processStep, Assembly executingAssembly)
        {
            _elementsByCaseInsensitiveName = new Dictionary<string, IElement>();
            _elementsById = new Dictionary<int, IElement>();
            _metaData = new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();
            _processStepMetaData = new List<KeyValuePair<string, string>>();
            _processStep = processStep;
            _elementTypeCount = new Dictionary<string, int>();
            _relationTypeCount = new Dictionary<string, int>();
            _relations = new Dictionary<int, List<IRelation>>();
            AddMetaData("Executable", SystemInfo.GetExecutableInfo(executingAssembly));
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

        public void AddMetaData(string name, string value)
        {
            Logger.LogUserMessage($"Metadata: processStep={_processStep} name={name} value={value}");
            _processStepMetaData.Add(new KeyValuePair<string, string>(name, value));
        }

        public void Load(string dsiFilename)
        {
            AnalyzerLogger.LogDataModelAction("Load data model file=" + dsiFilename);

            CompressedFile modelFile = new CompressedFile(dsiFilename);
            modelFile.ReadFile(ReadDsiXml, null);
        }

        private void ReadDsiXml(Stream stream, IProgress<int> progress)
        {
            Dictionary<int, IElement> elementsById = new Dictionary<int, IElement>();

            XmlReader xReader = XmlReader.Create(stream);
            while (xReader.Read())
            {
                switch (xReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xReader.Name == "metadatagroup")
                        {
                            string groupName = xReader.GetAttribute("name");
                            List<KeyValuePair<string, string>> groupMetaData = new List<KeyValuePair<string, string>>();
                            _metaData.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(groupName, groupMetaData));

                            XmlReader xMetaDataReader = xReader.ReadSubtree();
                            while (xMetaDataReader.Read())
                            {
                                if (xMetaDataReader.Name == "metadata")
                                {
                                    string name = xMetaDataReader.GetAttribute("name");
                                    string value = xMetaDataReader.GetAttribute("value");
                                    if ((name != null) && (value != null))
                                    {
                                        groupMetaData.Add(new KeyValuePair<string, string>(name, value));
                                    }
                                }
                            }
                        }

                        if (xReader.Name == "element")
                        {
                            int eid;
                            int.TryParse(xReader.GetAttribute("id"), out eid);
                            string name = xReader.GetAttribute("name");
                            string type = xReader.GetAttribute("type");
                            string source = xReader.GetAttribute("source");
                            elementsById[eid] = AddElement(name, type, source);
                        }

                        if (xReader.Name == "relation")
                        {
                            int providerId;
                            int.TryParse(xReader.GetAttribute("providerId"), out providerId);

                            int consumerId;
                            int.TryParse(xReader.GetAttribute("consumerId"), out consumerId);

                            string type = xReader.GetAttribute("type");

                            int weight;
                            int.TryParse(xReader.GetAttribute("weight"), out weight);

                            IElement consumerType = elementsById.ContainsKey(consumerId) ? elementsById[consumerId] : null;
                            IElement providerType = elementsById.ContainsKey(providerId) ? elementsById[providerId] : null;

                            if (consumerType != null && providerType != null)
                            {
                                AddRelation(consumerType.Name, providerType.Name, type, weight, "Load model");
                            }
                        }
                        break;
                    case XmlNodeType.Text:
                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }
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

            _metaData.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(_processStep, _processStepMetaData));
            CompressedFile modelFile = new CompressedFile(dsiFilename);
            modelFile.WriteFile(WriteDsiXml, null, compressFile);
        }

        private void WriteDsiXml(Stream stream, IProgress<int> progress)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("  ")
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement("system", "urn:dsi-schema");
                {
                    foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> metaDataGroup in _metaData)
                    {
                        writer.WriteStartElement("metadatagroup");
                        writer.WriteAttributeString("name", metaDataGroup.Key);
                        foreach (KeyValuePair<string, string> metaData in metaDataGroup.Value)
                        {
                            writer.WriteStartElement("metadata");
                            writer.WriteAttributeString("name", metaData.Key);
                            writer.WriteAttributeString("value", metaData.Value);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteStartElement("elements");
                    foreach (IElement element in _elementsByCaseInsensitiveName.Values)
                    {
                        writer.WriteStartElement("element");
                        writer.WriteAttributeString("id", element.ElementId.ToString());
                        writer.WriteAttributeString("name", element.Name);
                        writer.WriteAttributeString("type", element.Type);
                        writer.WriteAttributeString("source", element.Source);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("relations");
                    foreach (IElement element in _elementsByCaseInsensitiveName.Values)
                    {
                        if (_relations.ContainsKey(element.ElementId))
                        {
                            foreach (IRelation relation in _relations[element.ElementId])
                            {
                                writer.WriteStartElement("relation");

                                writer.WriteAttributeString("consumerId", relation.ConsumerId.ToString());
                                writer.WriteAttributeString("providerId", relation.ProviderId.ToString());
                                writer.WriteAttributeString("type", relation.Type);
                                writer.WriteAttributeString("weight", relation.Weight.ToString());
                                writer.WriteEndElement();
                            }
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndDocument();
            }
        }

        public IElement AddElement(string name, string type, string source)
        {
            AnalyzerLogger.LogDataModelAction("Add element to data model name=" + name + " type=" + type + " source=" + source);

            string key = name.ToLower();
            if (!_elementsByCaseInsensitiveName.ContainsKey(key))
            {
                IncrementElementTypeCount(type);
                int id = _elementsByCaseInsensitiveName.Count;
                Element element = new Element(id, name, type, source);
                _elementsByCaseInsensitiveName[key] = element;
                _elementsById[id] = element;
                return element;
            }
            else
            {
                return null;
            }
        }

        private void IncrementElementTypeCount(string type)
        {
            if (!_elementTypeCount.ContainsKey(type))
            {
                _elementTypeCount[type] = 0;
            }
            _elementTypeCount[type]++;
        }

        public void RemoveElement(IElement element)
        {
            string key = element.Name.ToLower();
            _elementsByCaseInsensitiveName.Remove(key);
            _elementsById.Remove(element.ElementId);
        }

        /// <summary>
        /// Removed relations from and to element which have bee removed.
        /// For performance reasons it is done once after one or more
        /// element removal actions.
        /// </summary>
        public void Cleanup()
        {
            IElement[] elements = _elementsByCaseInsensitiveName.Values.ToArray();

            foreach (IElement element in elements)
            {
                if (_relations.ContainsKey(element.ElementId))
                {
                    IRelation[] relations = _relations[element.ElementId].ToArray();

                    foreach (IRelation relation in relations)
                    {
                        if (!_elementsById.ContainsKey(relation.ConsumerId) ||
                            !_elementsById.ContainsKey(relation.ProviderId))
                        {
                            _relations[element.ElementId].Remove(relation);
                        }
                    }
                }
            }
        }

        public void RenameElement(IElement element, string newName)
        {
            AnalyzerLogger.LogDataModelAction("Rename element in data model from name=" + element.Name + " to name=" + newName);
            Element e = element as Element;
            if (e != null)
            {
                string oldKey = e.Name.ToLower();
                _elementsByCaseInsensitiveName.Remove(oldKey);
                e.Name = newName;
                string newKey = e.Name.ToLower();
                _elementsByCaseInsensitiveName[newKey] = e;
            }
        }

        public IRelation AddRelation(string consumerName, string providerName, string type, int weight, string context)
        {
            AnalyzerLogger.LogDataModelAction("Add relation " + type + " from consumer=" + consumerName + " to provider=" + providerName + " in " + context);
            _relationCount++;

            Element consumer = FindElement(consumerName) as Element;
            IElement provider = FindElement(providerName);
            IRelation relation = null;

            if (consumer != null && provider != null)
            {
                IncrementRelationTypeCount(type);

                relation = new Relation(provider.ElementId, consumer.ElementId, type, weight);
                if (!_relations.ContainsKey(consumer.ElementId))
                {
                    _relations[consumer.ElementId] = new List<IRelation>();
                }

                _relations[consumer.ElementId].Add(relation);
            }
            else
            {
                AnalyzerLogger.LogDataModelRelationNotResolved(consumerName, providerName);
            }

            return relation;
        }

        private void IncrementRelationTypeCount(string type)
        {
            if (!_relationTypeCount.ContainsKey(type))
            {
                _relationTypeCount[type] = 0;
            }
            _relationTypeCount[type]++;
        }

        public void SkipRelation(string consumerName, string providerName, string type, string context)
        {
            AnalyzerLogger.LogDataModelAction("Skip relation " + type + " from consumer=" + consumerName + " to provider=" + providerName + " in " + context);

            AnalyzerLogger.LogDataModelRelationNotResolved(consumerName, providerName);
            _relationCount++;
        }

        public ICollection<IElement> Elements => _elementsByCaseInsensitiveName.Values;

        public int TotalElementCount => _elementsByCaseInsensitiveName.Values.Count;

        public IElement FindElement(string name)
        {
            string key = name.ToLower();
            return _elementsByCaseInsensitiveName.ContainsKey(key) ? _elementsByCaseInsensitiveName[key] : null;
        }

        public IElement FindElement(int id)
        {
            return _elementsById.ContainsKey(id) ? _elementsById[id] : null;
        }

        public ICollection<IRelation> GetProviderRelations(IElement consumer)
        {
            if (_relations.ContainsKey(consumer.ElementId))
            {
                return _relations[consumer.ElementId];
            }
            else
            {
                return new List<IRelation>();
            }
        }

        public bool DoesRelationExist(IElement consumer, IElement provider)
        {
            bool doesRelationExist = false;

            if (_relations.ContainsKey(consumer.ElementId))
            {
                foreach (IRelation relation in _relations[consumer.ElementId])
                {
                    if (relation.ProviderId == provider.ElementId)
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

                foreach (IElement element in _elementsByCaseInsensitiveName.Values)
                {
                    if (_relations.ContainsKey(element.ElementId))
                    {
                        count += _relations[element.ElementId].Count;
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
    }
}

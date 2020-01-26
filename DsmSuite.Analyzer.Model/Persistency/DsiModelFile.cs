using System;
using System.IO;
using System.Xml;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Model.Persistency;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public class DsiModelFile
    {
        private const string RootXmlNode = "dsimodel";
        private const string ModelElementCountXmlAttribute = "elementCount";
        private const string ModelRelationCountXmlAttribute = "relationCount";

        private const string MetaDataGroupXmlNode = "metadatagroup";
        private const string MetaDataGroupNameXmlAttribute = "name";
        private const string MetaDataXmlNode = "metadata";
        private const string MetaDataItemNameXmlAttribute = "name";
        private const string MetaDataItemValueXmlAttribute = "value";

        private const string ElementGroupXmlNode = "elements";

        private const string ElementXmlNode = "element";
        private const string ElementIdXmlAttribute = "id";
        private const string ElementNameXmlAttribute = "name";
        private const string ElementTypeXmlAttribute = "type";
        private const string ElementSourceXmlAttribute = "source";

        private const string RelationGroupXmlNode = "relations";

        private const string RelationXmlNode = "relation";
        private const string RelationFromXmlAttribute = "from";
        private const string RelationToXmlAttribute = "to";
        private const string RelationTypeXmlAttribute = "type";
        private const string RelationWeightXmlAttribute = "weight";

        private readonly string _filename;
        private readonly IMetaDataModelFileCallback _metaDataModelCallback;
        private readonly IDsiElementModelFileCallback _elementModelCallback;
        private readonly IDsiRelationModelFileCallback _relationModelCallback;

        private int _totalElementCount;
        private int _progressedElementCount;
        private int _totalRelationCount;
        private int _progressedRelationCount;
        private int _progressPercentage;
        private string _progressActionText;

        public DsiModelFile(string filename, 
                            IMetaDataModelFileCallback metaDataModelCallback,
                            IDsiElementModelFileCallback elementModelCallback,
                            IDsiRelationModelFileCallback relationModelCallback)
        {
            _filename = filename;
            _metaDataModelCallback = metaDataModelCallback;
            _elementModelCallback = elementModelCallback;
            _relationModelCallback = relationModelCallback;
        }

        public void Save(bool compressed, IProgress<ProgressInfo> progress)
        {
            _progressActionText = "Saving dsi model";
            CompressedFile<ProgressInfo> modelFile = new CompressedFile<ProgressInfo>(_filename);
            modelFile.WriteFile(WriteDsiXml, progress, compressed);
        }

        public void Load(IProgress<ProgressInfo> progress)
        {
            _progressActionText = "Loading dsi model";
            CompressedFile<ProgressInfo> modelFile = new CompressedFile<ProgressInfo>(_filename);
            modelFile.ReadFile(ReadDsiXml, progress);
        }

        private void WriteDsiXml(Stream stream, IProgress<ProgressInfo> progress)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("  ")
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();

                writer.WriteStartElement(RootXmlNode, "urn:dsi-schema");
                {
                    WriteModelAttributes(writer);
                    WriteMetaData(writer);
                    WriteElements(writer, progress);
                    WriteRelations(writer, progress);
                }
                writer.WriteEndDocument();
            }
        }

        private void ReadDsiXml(Stream stream, IProgress<ProgressInfo> progress)
        {
            XmlReader xReader = XmlReader.Create(stream);
            while (xReader.Read())
            {
                switch (xReader.NodeType)
                {
                    case XmlNodeType.Element:
                        ReadModelAttributes(xReader);
                        ReadMetaDataGroup(xReader);
                        ReadElement(xReader, progress);
                        ReadRelation(xReader, progress);
                        break;
                    case XmlNodeType.Text:
                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }
        }

        private void WriteModelAttributes(XmlWriter writer)
        {
            _totalElementCount = _elementModelCallback.GetElementCount();
            writer.WriteAttributeString(ModelElementCountXmlAttribute, _totalElementCount.ToString());
            _progressedElementCount = 0;

            _totalRelationCount = _relationModelCallback.GetRelationCount();
            writer.WriteAttributeString(ModelRelationCountXmlAttribute, _totalRelationCount.ToString());
            _progressedRelationCount = 0;
        }

        private void ReadModelAttributes(XmlReader xReader)
        {
            if (xReader.Name == RootXmlNode)
            {
                int? elementCount = ParseInt(xReader.GetAttribute(ModelElementCountXmlAttribute));
                int? relationCount = ParseInt(xReader.GetAttribute(ModelRelationCountXmlAttribute));

                if (elementCount.HasValue && relationCount.HasValue)
                {
                    _totalElementCount = elementCount.Value;
                    _progressedElementCount = 0;
                    _totalRelationCount = relationCount.Value;
                    _progressedRelationCount = 0;
                }
            }
        }

        private void WriteMetaData(XmlWriter writer)
        {
            foreach (string group in _metaDataModelCallback.GetExportedMetaDataGroups())
            {
                WriteMetaDataGroup(writer, group);
            }
        }

        private void WriteMetaDataGroup(XmlWriter writer, string group)
        {
            writer.WriteStartElement(MetaDataGroupXmlNode);
            writer.WriteAttributeString(MetaDataGroupNameXmlAttribute, group);

            foreach (IMetaDataItem metaDataItem in _metaDataModelCallback.GetExportedMetaDataGroupItems(group))
            {
                writer.WriteStartElement(MetaDataXmlNode);
                writer.WriteAttributeString(MetaDataItemNameXmlAttribute, metaDataItem.Name);
                writer.WriteAttributeString(MetaDataItemValueXmlAttribute, metaDataItem.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ReadMetaDataGroup(XmlReader xReader)
        {
            if (xReader.Name == MetaDataGroupXmlNode)
            {
                string group = xReader.GetAttribute(MetaDataGroupNameXmlAttribute);
                XmlReader xMetaDataReader = xReader.ReadSubtree();
                while (xMetaDataReader.Read())
                {
                    if (xMetaDataReader.Name == MetaDataXmlNode)
                    {
                        string name = xMetaDataReader.GetAttribute(MetaDataItemNameXmlAttribute);
                        string value = xMetaDataReader.GetAttribute(MetaDataItemValueXmlAttribute);
                        if ((name != null) && (value != null))
                        {
                            _metaDataModelCallback.ImportMetaDataItem(group, name, value);
                        }
                    }
                }
            }
        }

        private void WriteElements(XmlWriter writer, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(ElementGroupXmlNode);
            foreach (IDsiElement element in _elementModelCallback.GetElements())
            {
                WriteElement(writer, element, progress);
            }
            writer.WriteEndElement();
        }

        private void WriteElement(XmlWriter writer, IDsiElement element, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(ElementXmlNode);
            writer.WriteAttributeString(ElementIdXmlAttribute, element.Id.ToString());
            writer.WriteAttributeString(ElementNameXmlAttribute, element.Name);
            writer.WriteAttributeString(ElementTypeXmlAttribute, element.Type);
            writer.WriteAttributeString(ElementSourceXmlAttribute, element.Source);
            writer.WriteEndElement();

            _progressedElementCount++;
            UpdateProgress(progress);
        }

        private void ReadElement(XmlReader xReader, IProgress<ProgressInfo> progress)
        {
            if (xReader.Name == ElementXmlNode)
            {
                int? id = ParseInt(xReader.GetAttribute(ElementIdXmlAttribute));
                string name = xReader.GetAttribute(ElementNameXmlAttribute);
                string type = xReader.GetAttribute(ElementTypeXmlAttribute);
                string source = xReader.GetAttribute(ElementSourceXmlAttribute);

                if (id.HasValue)
                {
                    _elementModelCallback.ImportElement(id.Value, name, type, source);
                }

                _progressedElementCount++;
                UpdateProgress(progress);
            }
        }

        private void WriteRelations(XmlWriter writer, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(RelationGroupXmlNode);
            foreach (IDsiRelation relation in _relationModelCallback.GetRelations())
            {
                WriteRelation(writer, relation, progress);
            }
            writer.WriteEndElement();
        }

        private void WriteRelation(XmlWriter writer, IDsiRelation relation, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(RelationXmlNode);
            writer.WriteAttributeString(RelationFromXmlAttribute, relation.ConsumerId.ToString());
            writer.WriteAttributeString(RelationToXmlAttribute, relation.ProviderId.ToString());
            writer.WriteAttributeString(RelationTypeXmlAttribute, relation.Type);
            writer.WriteAttributeString(RelationWeightXmlAttribute, relation.Weight.ToString());
            writer.WriteEndElement();

            _progressedRelationCount++;
            UpdateProgress(progress);
        }

        private void ReadRelation(XmlReader xReader, IProgress<ProgressInfo> progress)
        {
            if (xReader.Name == RelationXmlNode)
            {
                int? consumerId = ParseInt(xReader.GetAttribute(RelationFromXmlAttribute));
                int? providerId = ParseInt(xReader.GetAttribute(RelationToXmlAttribute));
                string type = xReader.GetAttribute(RelationTypeXmlAttribute);
                int? weight = ParseInt(xReader.GetAttribute(RelationWeightXmlAttribute));

                if (consumerId.HasValue && providerId.HasValue && weight.HasValue)
                {
                    _relationModelCallback.ImportRelation(consumerId.Value, providerId.Value, type, weight.Value);
                }

                _progressedRelationCount++;
                UpdateProgress(progress);
            }
        }

        private void UpdateProgress(IProgress<ProgressInfo> progress)
        {
            if (progress != null)
            {
                int totalItemCount = _totalElementCount + _totalRelationCount;
                int progressedItemCount = _progressedElementCount + _progressedRelationCount;

                int currentProgressPercentage = 0;
                if (totalItemCount > 0)
                {
                    currentProgressPercentage = progressedItemCount * 100 / totalItemCount;
                }

                if (_progressPercentage != currentProgressPercentage)
                {
                    _progressPercentage = currentProgressPercentage;

                    ProgressInfo progressInfoInfo = new ProgressInfo
                    {
                        ActionText = _progressActionText,
                        Percentage = currentProgressPercentage,
                        TotalItemCount = totalItemCount,
                        CurrentItemCount = progressedItemCount,
                        ItemType = "items",
                        Done = currentProgressPercentage == 100
                    };

                    progress.Report(progressInfoInfo);
                }
            }
        }

        private int? ParseInt(string value)
        {
            int? result = null;

            int parsedValued;
            if (int.TryParse(value, out parsedValued))
            {
                result = parsedValued;
            }
            return result;
        }
    }
}

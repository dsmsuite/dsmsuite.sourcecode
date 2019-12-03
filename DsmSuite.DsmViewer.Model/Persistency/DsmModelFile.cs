using System;
using System.IO;
using System.Xml;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public class DsmModelFile
    {
        private const string RootXmlNode = "model";
        private const string ModelelementCountXmlAttribute = "elementCount";
        private const string ModelrelationCountXmlAttribute = "relationCount";

        private const string MetaDataGroupXmlNode = "metadatagroup";
        private const string MetaDataGroupNameXmlAttribute = "name";

        private const string MetaDataXmlNode = "metadata";
        private const string MetaDataItemNameXmlAttribute = "name";
        private const string MetaDataItemValueXmlAttribute = "value";

        private const string ElementGroupXmlNode = "elements";

        private const string ElementXmlNode = "element";
        private const string ElementIdXmlAttribute = "id";
        private const string ElementOrderXmlAttribute = "order";
        private const string ElementNameXmlAttribute = "name";
        private const string ElementTypeXmlAttribute = "type";
        private const string ElementExpandedXmlAttribute = "expanded";
        private const string ElementParentXmlAttribute  = "parent";

        private const string RelationGroupXmlNode = "relations";

        private const string RelationXmlNode = "relation";
        private const string RelationConsumerIdXmlAttribute = "consumerId";
        private const string RelationProviderIdXmlAttribute = "providerId";
        private const string RelationTypeXmlAttribute = "type";
        private const string RelationWeightXmlAttribute = "weight";

        private readonly string _filename;
        private readonly IDsmModelFileCallback _callback;
        private int _totalElementCount;
        private int _totalRelationCount;
        private int _totalItemCount;
        private int _progressItemCount;
        private int _progress;

        public DsmModelFile(string filename, IDsmModelFileCallback callback)
        {
            _filename = filename;
            _callback = callback;
        }

        public void Save(bool compressed, IProgress<DsmProgressInfo> progress)
        {
            CompressedFile<DsmProgressInfo> modelFile = new CompressedFile<DsmProgressInfo>(_filename);
            modelFile.WriteFile(WriteDsmXml, progress, compressed);
        }

        public void Load(IProgress<DsmProgressInfo> progress)
        {
            CompressedFile<DsmProgressInfo> modelFile = new CompressedFile<DsmProgressInfo>(_filename);
            modelFile.ReadFile(ReadDsmXml, progress);
        }

        public bool IsCompressedFile()
        {
            CompressedFile<DsmProgressInfo> modelFile = new CompressedFile<DsmProgressInfo>(_filename);
            return modelFile.IsCompressed;
        }

        private void WriteDsmXml(Stream stream, IProgress<DsmProgressInfo> progress)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("  ")
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(RootXmlNode);
                {
                    WriteModelAttributes(writer);
                    WriteMetaData(writer);
                    WriteElements(writer, progress);
                    WriteRelations(writer, progress);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void ReadDsmXml(Stream stream, IProgress<DsmProgressInfo> progress)
        {
            using (XmlReader xReader = XmlReader.Create(stream))
            {
                while (xReader.Read())
                {
                    switch (xReader.NodeType)
                    {
                        case XmlNodeType.Element:

                            ReadModelAttributes(xReader);
                            ReadMetaData(xReader);
                            ReadElements(xReader, progress);
                            ReadRelations(xReader, progress);
                            break;
                        case XmlNodeType.Text:
                            break;
                        case XmlNodeType.EndElement:
                            break;
                    }
                }
            }
        }

        private void WriteModelAttributes(XmlWriter writer)
        {
            _totalElementCount = _callback.GetElements().Count;
            writer.WriteAttributeString(ModelelementCountXmlAttribute, _totalElementCount.ToString());

            _totalRelationCount = _callback.GetRelations().Count;
            writer.WriteAttributeString(ModelrelationCountXmlAttribute, _totalRelationCount.ToString());

            _totalItemCount = _totalElementCount + _totalRelationCount;
        }

        private void ReadModelAttributes(XmlReader xReader)
        {
            if (xReader.Name == RootXmlNode)
            {
                int? elementCount = ParseInt(xReader.GetAttribute(ModelelementCountXmlAttribute));
                int? relationCount = ParseInt(xReader.GetAttribute(ModelrelationCountXmlAttribute));

                if (elementCount.HasValue && relationCount.HasValue)
                {
                    _totalElementCount = elementCount.Value;
                    _totalRelationCount = relationCount.Value;
                    _totalItemCount = _totalElementCount + _totalRelationCount;
                    _progressItemCount = 0;
                }
            }
        }

        private void WriteMetaData(XmlWriter writer)
        {
            foreach (string groupName in _callback.GetMetaDataGroups())
            {
                writer.WriteStartElement(MetaDataGroupXmlNode);
                writer.WriteAttributeString(MetaDataGroupNameXmlAttribute, groupName);

                foreach (IDsmMetaDataItem metaDataItem in _callback.GetMetaDataGroupItems(groupName))
                {
                    writer.WriteStartElement(MetaDataXmlNode);
                    writer.WriteAttributeString(MetaDataItemNameXmlAttribute, metaDataItem.Name);
                    writer.WriteAttributeString(MetaDataItemValueXmlAttribute, metaDataItem.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void ReadMetaData(XmlReader xReader)
        {
            if (xReader.Name == MetaDataGroupXmlNode)
            {
                string groupName = xReader.GetAttribute(MetaDataGroupNameXmlAttribute);
                XmlReader xMetaDataReader = xReader.ReadSubtree();
                while (xMetaDataReader.Read())
                {
                    if (xMetaDataReader.Name == MetaDataXmlNode)
                    {
                        string name = xMetaDataReader.GetAttribute(MetaDataItemNameXmlAttribute);
                        string value = xMetaDataReader.GetAttribute(MetaDataItemValueXmlAttribute);
                        if ((name != null) && (value != null))
                        {
                            _callback.ImportMetaDataItem(groupName, name, value);
                        }
                    }
                }
            }
        }

        private void WriteElements(XmlWriter writer, IProgress<DsmProgressInfo> progress)
        {
            writer.WriteStartElement(ElementGroupXmlNode);
            foreach (IDsmElement child in _callback.GetElements())
            {
                WriteElementData(writer, child, progress);
            }
            writer.WriteEndElement();
        }
        
        private void ReadElements(XmlReader xReader, IProgress<DsmProgressInfo> progress)
        {
            if (xReader.Name == ElementXmlNode)
            {
                int? id = ParseInt(xReader.GetAttribute(ElementIdXmlAttribute));
                int? order = ParseInt(xReader.GetAttribute(ElementOrderXmlAttribute));
                string name = xReader.GetAttribute(ElementNameXmlAttribute);
                string type = xReader.GetAttribute(ElementTypeXmlAttribute);
                bool expanded = ParseBool(xReader.GetAttribute(ElementExpandedXmlAttribute));
                int? parentId = ParseInt(xReader.GetAttribute(ElementParentXmlAttribute));

                if (id.HasValue && order.HasValue)
                {
                    _callback.ImportElement(id.Value, name, type, order.Value, expanded, parentId);
                }

                _progressItemCount++;
                UpdateProgress(progress);
            }
        }

        private void WriteRelations(XmlWriter writer, IProgress<DsmProgressInfo> progress)
        {
            writer.WriteStartElement(RelationGroupXmlNode);
            foreach (IDsmRelation relation in _callback.GetRelations())
            {
                WriteRelationData(writer, relation, progress);
            }
            writer.WriteEndElement();
        }

        private void ReadRelations(XmlReader xReader, IProgress<DsmProgressInfo> progress)
        {
            if (xReader.Name == RelationXmlNode)
            {
                int? consumerId = ParseInt(xReader.GetAttribute(RelationConsumerIdXmlAttribute));
                int? providerId = ParseInt(xReader.GetAttribute(RelationProviderIdXmlAttribute));
                string type = xReader.GetAttribute(RelationTypeXmlAttribute);
                int? weight = ParseInt(xReader.GetAttribute(RelationWeightXmlAttribute));

                if (consumerId.HasValue &&
                    providerId.HasValue &&
                    weight.HasValue)
                {
                    _callback.ImportRelation(consumerId.Value, providerId.Value, type, weight.Value);
                }

                _progressItemCount++;
                UpdateProgress(progress);
            }
        }

        private void WriteElementData(XmlWriter writer, IDsmElement element, IProgress<DsmProgressInfo> progress)
        {
            writer.WriteStartElement(ElementXmlNode);
            writer.WriteAttributeString(ElementIdXmlAttribute, element.Id.ToString());
            writer.WriteAttributeString(ElementOrderXmlAttribute, element.Order.ToString());
            writer.WriteAttributeString(ElementNameXmlAttribute, element.Name);
            writer.WriteAttributeString(ElementTypeXmlAttribute, element.Type);
            writer.WriteAttributeString(ElementExpandedXmlAttribute, element.IsExpanded.ToString());
            if (element.Parent != null)
            {
                writer.WriteAttributeString(ElementParentXmlAttribute, element.Parent.Id.ToString());
            }
            writer.WriteEndElement();

            _progressItemCount++;
            UpdateProgress(progress);

            foreach (IDsmElement child in element.Children)
            {
                WriteElementData(writer, child, progress);
            }
        }

        private void WriteRelationData(XmlWriter writer, IDsmRelation relation, IProgress<DsmProgressInfo> progress)
        {
            _progressItemCount++;
            UpdateProgress(progress);

            writer.WriteStartElement(RelationXmlNode);
            writer.WriteAttributeString(RelationConsumerIdXmlAttribute, relation.ConsumerId.ToString());
            writer.WriteAttributeString(RelationProviderIdXmlAttribute, relation.ProviderId.ToString());
            writer.WriteAttributeString(RelationTypeXmlAttribute, relation.Type);
            writer.WriteAttributeString(RelationWeightXmlAttribute, relation.Weight.ToString());
            writer.WriteEndElement();
        }

        private void UpdateProgress(IProgress<DsmProgressInfo> progress)
        {
            if (progress != null)
            {
                int currentProgress = 0;
                if (_totalItemCount > 0)
                {
                    currentProgress = _progressItemCount * 100 / _totalItemCount;
                }

                if (_progress != currentProgress)
                {
                    _progress = currentProgress;

                    DsmProgressInfo progressInfoInfo = new DsmProgressInfo
                    {
                        ElementCount = _totalElementCount,
                        RelationCount = _totalRelationCount,
                        Progress = _progress
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

        private bool ParseBool(string value)
        {
            bool result = false;

            bool parsedValued;
            if (bool.TryParse(value, out parsedValued))
            {
                result = parsedValued;
            }
            return result;
        }
    }
}

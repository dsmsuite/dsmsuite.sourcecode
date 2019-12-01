using System;
using System.IO;
using System.Xml;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public class DsiModelFile
    {
        private const string MetaDataGroupXmlNodeName = "metadatagroup";
        private const string MetaDataGroupNameXmlAttributeName = "name";
        private const string MetaDataXmlNodeName = "metadata";
        private const string MetaDataItemNameXmlAttributeName = "name";
        private const string MetaDataItemValueXmlAttributeName = "value";

        private const string ElementGroupXmlNodeName = "elements";
        private const string ElementXmlNodeName = "element";
        private const string ElementIdXmlAttributeName = "id";
        private const string ElementNameXmlAttributeName = "name";
        private const string ElementTypeXmlAttributeName = "type";
        private const string ElementSourceXmlAttributeName = "source";

        private const string RelationGroupXmlNodeName = "relations";
        private const string RelationXmlNodeName = "relation";

        private const string RelationConsumerIdXmlAttributeName = "consumerId";
        private const string RelationProviderIdmlAttributeName = "providerId";
        private const string RelationTypeXmlAttributeName = "type";
        private const string RelationWeightXmlAttributeName = "weight";

        private readonly string _filename;
        private readonly IDsiModelFileCallback _callback;

        public DsiModelFile(string filename, IDsiModelFileCallback callback)
        {
            _filename = filename;
            _callback = callback;
        }

        public void Save(bool compressed, IProgress<int> progress)
        {
            CompressedFile modelFile = new CompressedFile(_filename);
            modelFile.WriteFile(WriteDsiXml, progress, compressed);
        }

        public void Load(IProgress<int> progress)
        {
            CompressedFile modelFile = new CompressedFile(_filename);
            modelFile.ReadFile(ReadDsiXml, progress);
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
                    WriteMetaData(writer);
                    WriteElements(writer);
                    WriteRelations(writer);
                }
                writer.WriteEndDocument();
            }
        }

        private void ReadDsiXml(Stream stream, IProgress<int> progress)
        {
            XmlReader xReader = XmlReader.Create(stream);
            while (xReader.Read())
            {
                switch (xReader.NodeType)
                {
                    case XmlNodeType.Element:
                        ReadMetaData(xReader);
                        ReadElement(xReader);
                        ReadRelation(xReader);
                        break;
                    case XmlNodeType.Text:
                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }
        }
        
        private void WriteMetaData(XmlWriter writer)
        {
            foreach (string groupName in _callback.GetMetaDataGroups())
            {
                writer.WriteStartElement(MetaDataGroupXmlNodeName);
                writer.WriteAttributeString(MetaDataGroupNameXmlAttributeName, groupName);

                foreach (IMetaDataItem metaDataItem in _callback.GetMetaDataGroupItems(groupName))
                {
                    writer.WriteStartElement(MetaDataXmlNodeName);
                    writer.WriteAttributeString(MetaDataItemNameXmlAttributeName, metaDataItem.Name);
                    writer.WriteAttributeString(MetaDataItemValueXmlAttributeName, metaDataItem.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void ReadMetaData(XmlReader xReader)
        {
            if (xReader.Name == MetaDataGroupXmlNodeName)
            {
                string groupName = xReader.GetAttribute(MetaDataGroupNameXmlAttributeName);
                XmlReader xMetaDataReader = xReader.ReadSubtree();
                while (xMetaDataReader.Read())
                {
                    if (xMetaDataReader.Name == MetaDataXmlNodeName)
                    {
                        string name = xMetaDataReader.GetAttribute(MetaDataItemNameXmlAttributeName);
                        string value = xMetaDataReader.GetAttribute(MetaDataItemValueXmlAttributeName);
                        if ((name != null) && (value != null))
                        {
                            MetaDataItem metaDataItem = new MetaDataItem(name, value);
                            _callback.ImportMetaDataItem(groupName, metaDataItem);
                        }
                    }
                }
            }
        }
        
        private void WriteElements(XmlWriter writer)
        {
            writer.WriteStartElement(ElementGroupXmlNodeName);
            foreach (IElement element in _callback.GetElements())
            {
                writer.WriteStartElement(ElementXmlNodeName);
                writer.WriteAttributeString(ElementIdXmlAttributeName, element.Id.ToString());
                writer.WriteAttributeString(ElementNameXmlAttributeName, element.Name);
                writer.WriteAttributeString(ElementTypeXmlAttributeName, element.Type);
                writer.WriteAttributeString(ElementSourceXmlAttributeName, element.Source);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ReadElement(XmlReader xReader)
        {
            if (xReader.Name == ElementXmlNodeName)
            {
                int id;
                int.TryParse(xReader.GetAttribute(ElementIdXmlAttributeName), out id);
                string name = xReader.GetAttribute(ElementNameXmlAttributeName);
                string type = xReader.GetAttribute(ElementTypeXmlAttributeName);
                string source = xReader.GetAttribute(ElementSourceXmlAttributeName);
                Element element = new Element(id, name, type, source);
                _callback.ImportElement(element);
            }
        }

        private void WriteRelations(XmlWriter writer)
        {
            writer.WriteStartElement(RelationGroupXmlNodeName);
            foreach (IRelation relation in _callback.GetRelations())
            {
                writer.WriteStartElement(RelationXmlNodeName);
                writer.WriteAttributeString(RelationConsumerIdXmlAttributeName, relation.ConsumerId.ToString());
                writer.WriteAttributeString(RelationProviderIdmlAttributeName, relation.ProviderId.ToString());
                writer.WriteAttributeString(RelationTypeXmlAttributeName, relation.Type);
                writer.WriteAttributeString(RelationWeightXmlAttributeName, relation.Weight.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ReadRelation(XmlReader xReader)
        {
            if (xReader.Name == RelationXmlNodeName)
            {
                int consumerId;
                int.TryParse(xReader.GetAttribute(RelationConsumerIdXmlAttributeName), out consumerId);
                int providerId;
                int.TryParse(xReader.GetAttribute(RelationProviderIdmlAttributeName), out providerId);
                string type = xReader.GetAttribute(RelationTypeXmlAttributeName);
                int weight;
                int.TryParse(xReader.GetAttribute(RelationWeightXmlAttributeName), out weight);
                Relation relation = new Relation(consumerId, providerId, type, weight);
                _callback.ImportRelation(relation);
            }
        }
    }
}

using System;
using System.IO;
using System.Xml;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public class DsiModelFile
    {
        private const string RootXmlNode = "dsimodel";

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
        private readonly IDsiModelFileCallback _callback;

        public DsiModelFile(string filename, IDsiModelFileCallback callback)
        {
            _filename = filename;
            _callback = callback;
        }

        public void Save(bool compressed, IProgress<int> progress)
        {
            CompressedFile<int> modelFile = new CompressedFile<int>(_filename);
            modelFile.WriteFile(WriteDsiXml, progress, compressed);
        }

        public void Load(IProgress<int> progress)
        {
            CompressedFile<int> modelFile = new CompressedFile<int>(_filename);
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

                writer.WriteStartElement(RootXmlNode, "urn:dsi-schema");
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
                writer.WriteStartElement(MetaDataGroupXmlNode);
                writer.WriteAttributeString(MetaDataGroupNameXmlAttribute, groupName);

                foreach (IDsiMetaDataItem metaDataItem in _callback.GetMetaDataGroupItems(groupName))
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
        
        private void WriteElements(XmlWriter writer)
        {
            writer.WriteStartElement(ElementGroupXmlNode);
            foreach (IDsiElement element in _callback.GetElements())
            {
                writer.WriteStartElement(ElementXmlNode);
                writer.WriteAttributeString(ElementIdXmlAttribute, element.Id.ToString());
                writer.WriteAttributeString(ElementNameXmlAttribute, element.Name);
                writer.WriteAttributeString(ElementTypeXmlAttribute, element.Type);
                writer.WriteAttributeString(ElementSourceXmlAttribute, element.Source);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ReadElement(XmlReader xReader)
        {
            if (xReader.Name == ElementXmlNode)
            {
                int id;
                int.TryParse(xReader.GetAttribute(ElementIdXmlAttribute), out id);
                string name = xReader.GetAttribute(ElementNameXmlAttribute);
                string type = xReader.GetAttribute(ElementTypeXmlAttribute);
                string source = xReader.GetAttribute(ElementSourceXmlAttribute);
                _callback.ImportElement(id, name, type, source);
            }
        }

        private void WriteRelations(XmlWriter writer)
        {
            writer.WriteStartElement(RelationGroupXmlNode);
            foreach (IDsiRelation relation in _callback.GetRelations())
            {
                writer.WriteStartElement(RelationXmlNode);
                writer.WriteAttributeString(RelationFromXmlAttribute, relation.ConsumerId.ToString());
                writer.WriteAttributeString(RelationToXmlAttribute, relation.ProviderId.ToString());
                writer.WriteAttributeString(RelationTypeXmlAttribute, relation.Type);
                writer.WriteAttributeString(RelationWeightXmlAttribute, relation.Weight.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ReadRelation(XmlReader xReader)
        {
            if (xReader.Name == RelationXmlNode)
            {
                int consumerId;
                int.TryParse(xReader.GetAttribute(RelationFromXmlAttribute), out consumerId);
                int providerId;
                int.TryParse(xReader.GetAttribute(RelationToXmlAttribute), out providerId);
                string type = xReader.GetAttribute(RelationTypeXmlAttribute);
                int weight;
                int.TryParse(xReader.GetAttribute(RelationWeightXmlAttribute), out weight);

                _callback.ImportRelation(consumerId, providerId, type, weight);
            }
        }
    }
}

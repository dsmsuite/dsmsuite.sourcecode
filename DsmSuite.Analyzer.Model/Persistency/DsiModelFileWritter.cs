using System;
using System.IO;
using System.Xml;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public class DsiModelFileWritter
    {
        private readonly string _filename;
        private readonly IDsiModelFileWritterCallback _callback;

        public DsiModelFileWritter(string filename, IDsiModelFileWritterCallback callback)
        {
            _filename = filename;
            _callback = callback;
        }

        public void Save(bool compressed, IProgress<int> progress)
        {
            CompressedFile modelFile = new CompressedFile(_filename);
            modelFile.WriteFile(WriteDsiXml, progress, compressed);
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

        private void WriteMetaData(XmlWriter writer)
        {
            foreach (string groupName in _callback.GetMetaDataGroups())
            {
                writer.WriteStartElement("metadatagroup");
                writer.WriteAttributeString("name", groupName);

                foreach (IMetaDataItem metaDataItem in _callback.GetMetaDataItems(groupName))
                {
                    writer.WriteStartElement("metadata");
                    writer.WriteAttributeString("name", metaDataItem.Name);
                    writer.WriteAttributeString("value", metaDataItem.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void WriteElements(XmlWriter writer)
        {
            writer.WriteStartElement("elements");
            foreach (IElement element in _callback.GetElements())
            {
                writer.WriteStartElement("element");
                writer.WriteAttributeString("id", element.ElementId.ToString());
                writer.WriteAttributeString("name", element.Name);
                writer.WriteAttributeString("type", element.Type);
                writer.WriteAttributeString("source", element.Source);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void WriteRelations(XmlWriter writer)
        {
            writer.WriteStartElement("relations");
            foreach (IRelation relation in _callback.GetRelations())
            {
                writer.WriteStartElement("relation");
                writer.WriteAttributeString("consumerId", relation.ConsumerId.ToString());
                writer.WriteAttributeString("providerId", relation.ProviderId.ToString());
                writer.WriteAttributeString("type", relation.Type);
                writer.WriteAttributeString("weight", relation.Weight.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}

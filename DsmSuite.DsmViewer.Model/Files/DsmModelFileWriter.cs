using System.IO;
using System.Xml;
using DsmSuite.DsmViewer.Model.Dependencies;

namespace DsmSuite.DsmViewer.Model.Files
{
    public class DsmModelFileWriter : FileWriter
    {
        private readonly DependencyModel _dependencyModel;
        private readonly MetaData _metaData;

        public DsmModelFileWriter(string filename, DependencyModel dependencyModel, MetaData metaData) : base(filename)
        {
            _dependencyModel = dependencyModel;
            _metaData = metaData;
        }

        protected override void WriteContent(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = ("  ")
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("model");
                {
                    foreach (string group in _metaData.GetGroups())
                    {
                        WriteMetaData(writer, group);
                    }

                    writer.WriteStartElement("elements");
                    foreach (IElement child in _dependencyModel.RootElements)
                    {
                        WriteElementData(writer, child);
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("relations");
                    foreach (Relation relation in _dependencyModel.Relations)
                    {
                        WriteRelationData(writer, relation);
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void WriteMetaData(XmlWriter writer, string group)
        {
            writer.WriteStartElement("metadatagroup");
            writer.WriteAttributeString("name", group);
            foreach (string name in _metaData.GetNames(group))
            {
                string value = _metaData.GetValue(group, name);

                writer.WriteStartElement("metadata");
                writer.WriteAttributeString("name", name);
                writer.WriteAttributeString("value", value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private static void WriteElementData(XmlWriter writer, IElement element)
        {
            writer.WriteStartElement("element");
            writer.WriteAttributeString("id", element.Id.ToString());
            writer.WriteAttributeString("order", element.Order.ToString());
            writer.WriteAttributeString("name", element.Name);
            writer.WriteAttributeString("type", element.Type);
            writer.WriteAttributeString("expanded", element.IsExpanded.ToString());
            if (element.Parent != null)
            {
                writer.WriteAttributeString("parent", element.Parent.Id.ToString());
            }

            writer.WriteEndElement();

            foreach (IElement child in element.Children)
            {
                WriteElementData(writer, child);
            }

 
        }

        private static void WriteRelationData(XmlWriter writer, Relation relation)
        {
            writer.WriteStartElement("relation");
            writer.WriteAttributeString("from", relation.ConsumerId.ToString());
            writer.WriteAttributeString("to", relation.ProviderId.ToString());
            writer.WriteAttributeString("type", relation.Type);
            writer.WriteAttributeString("weight", relation.Weight.ToString());
            writer.WriteEndElement();
        }
    }
}

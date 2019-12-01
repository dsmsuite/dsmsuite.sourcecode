using System;
using System.IO;
using System.Xml;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Model.Persistency
{
    public class DsiModelFileReader
    {
        private readonly string _filename;
        private readonly IDsiModelFileReaderCallback _callback;

        public DsiModelFileReader(string filename, IDsiModelFileReaderCallback callback)
        {
            _filename = filename;
            _callback = callback;
        }

        public void Load(IProgress<int> progress)
        {
            CompressedFile modelFile = new CompressedFile(_filename);
            modelFile.ReadFile(ReadDsiXml, progress);
        }

        private void ReadDsiXml(Stream stream, IProgress<int> progress)
        {
            XmlReader xReader = XmlReader.Create(stream);
            while (xReader.Read())
            {
                switch (xReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (xReader.Name == "metadatagroup")
                        {
                            ReadMetaData(xReader);
                        }

                        if (xReader.Name == "element")
                        {
                            ReadElement(xReader);
                        }

                        if (xReader.Name == "relation")
                        {
                            ReadRelation(xReader);
                        }
                        break;
                    case XmlNodeType.Text:
                        break;
                    case XmlNodeType.EndElement:
                        break;
                }
            }
        }

        private void ReadMetaData(XmlReader xReader)
        {
            string groupName = xReader.GetAttribute("name");
            _callback.ReadMetaDataGroup(groupName);

            XmlReader xMetaDataReader = xReader.ReadSubtree();
            while (xMetaDataReader.Read())
            {
                if (xMetaDataReader.Name == "metadata")
                {
                    string name = xMetaDataReader.GetAttribute("name");
                    string value = xMetaDataReader.GetAttribute("value");
                    if ((name != null) && (value != null))
                    {
                        MetaDataItem metaDataItem = new MetaDataItem(name, value);
                        _callback.ReadMetaDataItem(groupName, metaDataItem);
                    }
                }
            }
        }
        
        private void ReadElement(XmlReader xReader)
        {
            int elementId;
            int.TryParse(xReader.GetAttribute("id"), out elementId);
            string name = xReader.GetAttribute("name");
            string type = xReader.GetAttribute("type");
            string source = xReader.GetAttribute("source");
            Element element = new Element(elementId, name, type, source);
            _callback.ReadElement(element);
        }

        private void ReadRelation(XmlReader xReader)
        {
            int providerId;
            int.TryParse(xReader.GetAttribute("providerId"), out providerId);

            int consumerId;
            int.TryParse(xReader.GetAttribute("consumerId"), out consumerId);

            string type = xReader.GetAttribute("type");

            int weight;
            int.TryParse(xReader.GetAttribute("weight"), out weight);

            Relation relation = new Relation(providerId, consumerId, type, weight);
            _callback.ReadRelation(relation);
        }
    }
}

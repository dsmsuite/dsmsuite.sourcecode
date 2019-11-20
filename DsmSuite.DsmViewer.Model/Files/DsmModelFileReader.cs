using System.IO;
using System.Xml;
using DsmSuite.DsmViewer.Model.Dependencies;

namespace DsmSuite.DsmViewer.Model.Files
{
    public class DsmModelFileReader : FileReader
    {
        private readonly DependencyModel _dependencyModel;
        private readonly MetaData _metaData;

        public DsmModelFileReader(string filename, DependencyModel dependencyModel, MetaData metaData) : base(filename)
        {
            _dependencyModel = dependencyModel;
            _metaData = metaData;
        }

        protected override void ReadContent(Stream stream)
        {
            using (XmlReader xReader = XmlReader.Create(stream))
            {
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
        }

        private void ReadMetaData(XmlReader xReader)
        {
            string group = xReader.GetAttribute("name");
            XmlReader xMetaDataReader = xReader.ReadSubtree();
            while (xMetaDataReader.Read())
            {
                if (xMetaDataReader.Name == "metadata")
                {
                    string name = xMetaDataReader.GetAttribute("name");
                    string value = xMetaDataReader.GetAttribute("value");
                    if ((name != null) && (value != null))
                    {
                        _metaData.AddMetaData(group, name, value);
                    }
                }
            }
        }

        private void ReadElement(XmlReader xReader)
        {
            int? id = ParseInt(xReader.GetAttribute("id"));
            int? order = ParseInt(xReader.GetAttribute("order"));
            string name = xReader.GetAttribute("name");
            string type = xReader.GetAttribute("type");
            bool expanded = ParseBool(xReader.GetAttribute("expanded"));
            int? parentId = ParseInt(xReader.GetAttribute("parent"));


            if (id.HasValue && order.HasValue)
            {
                _dependencyModel.AddElement(id.Value, name, type, order.Value, expanded, parentId);
            }
        }

        private void ReadRelation(XmlReader xReader)
        {
            int? consumerId = ParseInt(xReader.GetAttribute("from"));
            int? providerId = ParseInt(xReader.GetAttribute("to"));
            int? weight = ParseInt(xReader.GetAttribute("weight"));
            string type = xReader.GetAttribute("type");

            if ((consumerId.HasValue) &&
                (providerId.HasValue) &&
                (weight.HasValue))
            {
                _dependencyModel.AddRelation(consumerId.Value, providerId.Value, type, weight.Value);
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

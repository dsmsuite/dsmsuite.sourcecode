using System;
using System.IO;
using System.Xml;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Files.Base;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Files.Dsi
{
    public interface IDsiModelFileReaderCallback
    {
        void FoundMetaData(string group, string name, string value);
        void FoundElement(int id, string fullname, string type);

        void FoundRelation(int consumerId, int providerId,string type ,int weight);
    }

    public class DsiModelFileReader : FileReader
    {
        private readonly IDsiModelFileReaderCallback _callback;

        public DsiModelFileReader(string filename, IDsiModelFileReaderCallback callback) : base(filename)
        {
            _callback = callback;
        }

        protected override void ReadContent(Stream stream, IProgress<DsmProgressInfo> progress)
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
                        _callback.FoundMetaData(group, name, value);
                    }
                    else
                    {
                        Logger.LogError("Meta data could not be parsed");
                    }
                }
            }
        }

        private void ReadElement(XmlReader xReader)
        {
            int? id = ParseInt(xReader.GetAttribute("id"));
            string fullname = xReader.GetAttribute("name");
            string type = xReader.GetAttribute("type");

            if (id.HasValue)
            {
                _callback.FoundElement(id.Value, fullname, type);
            }
            else
            {
                Logger.LogError("Element could not be parsed");
            }
        }

        private void ReadRelation(XmlReader xReader)
        {
            int? consumerId = ParseInt(xReader.GetAttribute("consumerId"));
            int? providerId = ParseInt(xReader.GetAttribute("providerId"));
            string type = xReader.GetAttribute("type");
            int? weight = ParseInt(xReader.GetAttribute("weight"));

            if (consumerId.HasValue &&
                providerId.HasValue &&
                weight.HasValue)
            {
                _callback.FoundRelation(consumerId.Value, providerId.Value, type, weight.Value);
            }
            else
            {
                Logger.LogError("Relation could not be parsed");
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

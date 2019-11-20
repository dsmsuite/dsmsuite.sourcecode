using System;
using System.IO;
using System.Xml;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Files.Base;
using DsmSuite.DsmViewer.Util;

namespace DsmSuite.DsmViewer.Model.Files.Dsm
{
    public class DsmModelFileReader : FileReader
    {
        private readonly DependencyModel _dependencyModel;
        private readonly MetaData _metaData;
        private int _totalElementCount;
        private int _totalRelationCount;
        private int _totalItemCount;
        private int _readItemCount;
        private int _readItemProgress;

        public DsmModelFileReader(string filename, DependencyModel dependencyModel, MetaData metaData) : base(filename)
        {
            _dependencyModel = dependencyModel;
            _metaData = metaData;
        }

        protected override void ReadContent(Stream stream, IProgress<ProgressInfo> progress)
        {
            using (XmlReader xReader = XmlReader.Create(stream))
            {
                while (xReader.Read())
                {
                    switch (xReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xReader.Name == "model")
                            {
                                ReadModelData(xReader);
                            }

                            if (xReader.Name == "metadatagroup")
                            {
                                ReadMetaData(xReader);
                            }

                            if (xReader.Name == "element")
                            {
                                ReadElement(xReader, progress);
                            }

                            if (xReader.Name == "relation")
                            {
                                ReadRelation(xReader, progress);
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

        private void ReadModelData(XmlReader xReader)
        {
            int? elementCount = ParseInt(xReader.GetAttribute("elementCount"));
            int? relationCount = ParseInt(xReader.GetAttribute("relationCount"));
            if (elementCount.HasValue && relationCount.HasValue)
            {
                _totalElementCount = elementCount.Value;
                _totalRelationCount = relationCount.Value;
                _totalItemCount = _totalElementCount + _totalRelationCount;
                _readItemCount = 0;
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

        private void ReadElement(XmlReader xReader, IProgress<ProgressInfo> progress)
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

            _readItemCount++;
            UpdateProgress(progress);
        }

        private void ReadRelation(XmlReader xReader, IProgress<ProgressInfo> progress)
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

            _readItemCount++;
            UpdateProgress(progress);
        }

        private void UpdateProgress(IProgress<ProgressInfo> progress)
        {
            if (progress != null)
            {
                int readItemProgress = 0;
                if (_totalItemCount > 0)
                {
                    readItemProgress = _readItemCount * 100 / _totalItemCount;
                }

                if (_readItemProgress != readItemProgress)
                {
                    _readItemProgress = readItemProgress;

                    ProgressInfo progressInfoInfo = new ProgressInfo
                    {
                        ElementCount = _totalElementCount,
                        RelationCount = _totalRelationCount,
                        Progress = _readItemProgress
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

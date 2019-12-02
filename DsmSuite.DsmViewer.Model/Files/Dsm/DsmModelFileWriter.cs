using System;
using System.IO;
using System.Xml;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Files.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Model.Files.Dsm
{
    public class DsmModelFileWriter : FileWriter
    {
        private readonly DependencyModel _dependencyModel;
        private readonly MetaData _metaData;
        private int _totalElementCount;
        private int _totalRelationCount;
        private int _totalItemCount;
        private int _writtenItemCount;
        private int _writtenItemProgress;

        public DsmModelFileWriter(string filename, DependencyModel dependencyModel, MetaData metaData) : base(filename)
        {
            _dependencyModel = dependencyModel;
            _metaData = metaData;
        }

        protected override void WriteContent(Stream stream, IProgress<ProgressInfo> progress)
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
                    _totalElementCount = _dependencyModel.ElementCount;
                    writer.WriteAttributeString("elementCount", _totalElementCount.ToString());

                    _totalRelationCount = _dependencyModel.RelationCount;
                    writer.WriteAttributeString("relationCount", _totalRelationCount.ToString());

                    _totalItemCount = _totalElementCount + _totalRelationCount;

                    foreach (string group in _metaData.GetGroups())
                    {
                        WriteMetaData(writer, group);
                    }
                    
                    writer.WriteStartElement("elements");
                    foreach (IDsmElement child in _dependencyModel.RootElements)
                    {
                        WriteElementData(writer, child, progress);
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("relations");
                    foreach (DsmRelation relation in _dependencyModel.Relations)
                    {
                        WriteRelationData(writer, relation, progress);
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

        private void WriteElementData(XmlWriter writer, IDsmElement element, IProgress<ProgressInfo> progress)
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

            _writtenItemCount++;
            UpdateProgress(progress);

            foreach (IDsmElement child in element.Children)
            {
                WriteElementData(writer, child, progress);
            }
        }

        private void WriteRelationData(XmlWriter writer, DsmRelation relation, IProgress<ProgressInfo> progress)
        {
            _writtenItemCount++;
            UpdateProgress(progress);

            writer.WriteStartElement("relation");
            writer.WriteAttributeString("from", relation.ConsumerId.ToString());
            writer.WriteAttributeString("to", relation.ProviderId.ToString());
            writer.WriteAttributeString("type", relation.Type);
            writer.WriteAttributeString("weight", relation.Weight.ToString());
            writer.WriteEndElement();
        }

        private void UpdateProgress(IProgress<ProgressInfo> progress)
        {
            if (progress != null)
            {
                int writtenItemProgress = 0;
                if (_totalItemCount > 0)
                {
                    writtenItemProgress = _writtenItemCount * 100 / _totalItemCount;
                }

                if (_writtenItemProgress != writtenItemProgress)
                {
                    _writtenItemProgress = writtenItemProgress;

                    ProgressInfo progressInfoInfo = new ProgressInfo
                    {
                        ElementCount = _totalElementCount,
                        RelationCount = _totalRelationCount,
                        Progress = writtenItemProgress
                    };

                    progress.Report(progressInfoInfo);
                }
            }
        }
    }
}

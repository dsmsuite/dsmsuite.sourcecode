using System;
using System.IO;
using System.Xml;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.Common.Model.Interface;
using System.Collections.Generic;
using DsmSuite.Common.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Persistency
{
    public class DsmModelFile
    {
        private const string RootXmlNode = "dsmmodel";
        private const string ModelElementCountXmlAttribute = "elementCount";
        private const string ModelRelationCountXmlAttribute = "relationCount";
        private const string ModelActionCountXmlAttribute = "actionCount";

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
        private const string ElementParentXmlAttribute = "parent";
        private const string ElementDeletedXmlAttribute = "deleted";

        private const string RelationGroupXmlNode = "relations";

        private const string RelationXmlNode = "relation";
        private const string RelationIdXmlAttribute = "id";
        private const string RelationFromXmlAttribute = "from";
        private const string RelationToXmlAttribute = "to";
        private const string RelationTypeXmlAttribute = "type";
        private const string RelationWeightXmlAttribute = "weight";
        private const string RelationDeletedXmlAttribute = "deleted";

        private const string ActionGroupXmlNode = "actions";

        private const string ActionXmlNode = "action";
        private const string ActionIdXmlAttribute = "id";
        private const string ActionTypeXmlAttribute = "type";
        private const string ActionDataXmlNode = "data";
        private readonly string _filename;
        private readonly IMetaDataModelFileCallback _metaDataModelCallback;
        private readonly IDsmElementModelFileCallback _elementModelCallback;
        private readonly IDsmRelationModelFileCallback _relationModelCallback;
        private readonly IDsmActionModelFileCallback _actionModelCallback;
        private int _totalElementCount;
        private int _progressedElementCount;
        private int _totalRelationCount;
        private int _progressedRelationCount;
        private int _totalActionCount;
        private int _progressedActionCount;
        private int _progress;
        private string _progressActionText;

        public DsmModelFile(string filename,
                            IMetaDataModelFileCallback metaDataModelCallback,
                            IDsmElementModelFileCallback elementModelCallback,
                            IDsmRelationModelFileCallback relationModelCallback,
                            IDsmActionModelFileCallback actionModelCallback)
        {
            _filename = filename;
            _metaDataModelCallback = metaDataModelCallback;
            _elementModelCallback = elementModelCallback;
            _relationModelCallback = relationModelCallback;
            _actionModelCallback = actionModelCallback;
        }

        public void Save(bool compressed, IProgress<ProgressInfo> progress)
        {
            _progressActionText = "Saving dsm model";
            CompressedFile<ProgressInfo> modelFile = new CompressedFile<ProgressInfo>(_filename);
            modelFile.WriteFile(WriteDsmXml, progress, compressed);
        }

        public void Load(IProgress<ProgressInfo> progress)
        {
            _progressActionText = "Loading dsm model";
            CompressedFile<ProgressInfo> modelFile = new CompressedFile<ProgressInfo>(_filename);
            modelFile.ReadFile(ReadDsmXml, progress);
        }

        public bool IsCompressedFile()
        {
            CompressedFile<ProgressInfo> modelFile = new CompressedFile<ProgressInfo>(_filename);
            return modelFile.IsCompressed;
        }

        private void WriteDsmXml(Stream stream, IProgress<ProgressInfo> progress)
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
                    WriteActions(writer, progress);
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        private void ReadDsmXml(Stream stream, IProgress<ProgressInfo> progress)
        {
            using (XmlReader xReader = XmlReader.Create(stream))
            {
                while (xReader.Read())
                {
                    switch (xReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ReadModelAttributes(xReader);
                            ReadMetaDataGroup(xReader);
                            ReadElement(xReader, progress);
                            ReadRelation(xReader, progress);
                            ReadAction(xReader, progress);
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
            _totalElementCount = _elementModelCallback.GetExportedElementCount();
            writer.WriteAttributeString(ModelElementCountXmlAttribute, _totalElementCount.ToString());
            _progressedElementCount = 0;

            _totalRelationCount = _relationModelCallback.GetExportedRelationCount();
            writer.WriteAttributeString(ModelRelationCountXmlAttribute, _totalRelationCount.ToString());
            _progressedRelationCount = 0;

            _totalActionCount = _actionModelCallback.GetExportedActionCount();
            writer.WriteAttributeString(ModelActionCountXmlAttribute, _totalActionCount.ToString());
            _progressedActionCount = 0;
        }

        private void ReadModelAttributes(XmlReader xReader)
        {
            if (xReader.Name == RootXmlNode)
            {
                int? elementCount = ParseInt(xReader.GetAttribute(ModelElementCountXmlAttribute));
                int? relationCount = ParseInt(xReader.GetAttribute(ModelRelationCountXmlAttribute));
                int? actionCount = ParseInt(xReader.GetAttribute(ModelActionCountXmlAttribute));

                if (elementCount.HasValue && relationCount.HasValue && actionCount.HasValue)
                {
                    _totalElementCount = elementCount.Value;
                    _progressedElementCount = 0;
                    _totalRelationCount = relationCount.Value;
                    _progressedRelationCount = 0;
                    _totalActionCount = actionCount.Value;
                    _progressedActionCount = 0;
                }
            }
        }

        private void WriteMetaData(XmlWriter writer)
        {
            foreach (string group in _metaDataModelCallback.GetExportedMetaDataGroups())
            {
                WriteMetaDataGroup(writer, group);
            }
        }

        private void WriteMetaDataGroup(XmlWriter writer, string group)
        {
            writer.WriteStartElement(MetaDataGroupXmlNode);
            writer.WriteAttributeString(MetaDataGroupNameXmlAttribute, group);

            foreach (IMetaDataItem metaDataItem in _metaDataModelCallback.GetExportedMetaDataGroupItems(group))
            {
                writer.WriteStartElement(MetaDataXmlNode);
                writer.WriteAttributeString(MetaDataItemNameXmlAttribute, metaDataItem.Name);
                writer.WriteAttributeString(MetaDataItemValueXmlAttribute, metaDataItem.Value);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ReadMetaDataGroup(XmlReader xReader)
        {
            if (xReader.Name == MetaDataGroupXmlNode)
            {
                string group = xReader.GetAttribute(MetaDataGroupNameXmlAttribute);
                XmlReader xMetaDataReader = xReader.ReadSubtree();
                while (xMetaDataReader.Read())
                {
                    if (xMetaDataReader.Name == MetaDataXmlNode)
                    {
                        string name = xMetaDataReader.GetAttribute(MetaDataItemNameXmlAttribute);
                        string value = xMetaDataReader.GetAttribute(MetaDataItemValueXmlAttribute);
                        if ((name != null) && (value != null))
                        {
                            _metaDataModelCallback.ImportMetaDataItem(group, name, value);
                        }
                    }
                }
            }
        }

        private void WriteElements(XmlWriter writer, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(ElementGroupXmlNode);
            foreach (IDsmElement element in _elementModelCallback.GetRootElement().ExportedChildren)
            {
                WriteElement(writer, element, progress);
            }
            writer.WriteEndElement();
        }

        private void WriteElement(XmlWriter writer, IDsmElement element, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(ElementXmlNode);
            writer.WriteAttributeString(ElementIdXmlAttribute, element.Id.ToString());
            writer.WriteAttributeString(ElementOrderXmlAttribute, element.Order.ToString());
            writer.WriteAttributeString(ElementNameXmlAttribute, element.Name);
            writer.WriteAttributeString(ElementTypeXmlAttribute, element.Type);
            writer.WriteAttributeString(ElementExpandedXmlAttribute, element.IsExpanded.ToString());
            if (element.IsDeleted)
            {
                writer.WriteAttributeString(ElementDeletedXmlAttribute, "true");
            }
            if ((element.Parent != null) && (element.Parent.Id > 0))
            {
                writer.WriteAttributeString(ElementParentXmlAttribute, element.Parent.Id.ToString());
            }
            writer.WriteEndElement();

            _progressedElementCount++;
            UpdateProgress(progress);

            foreach (IDsmElement child in element.ExportedChildren)
            {
                WriteElement(writer, child, progress);
            }
        }

        private void ReadElement(XmlReader xReader, IProgress<ProgressInfo> progress)
        {
            if (xReader.Name == ElementXmlNode)
            {
                int? id = ParseInt(xReader.GetAttribute(ElementIdXmlAttribute));
                int? order = ParseInt(xReader.GetAttribute(ElementOrderXmlAttribute));
                string name = xReader.GetAttribute(ElementNameXmlAttribute);
                string type = xReader.GetAttribute(ElementTypeXmlAttribute);
                bool expanded = ParseBool(xReader.GetAttribute(ElementExpandedXmlAttribute));
                int? parent = ParseInt(xReader.GetAttribute(ElementParentXmlAttribute));
                bool deleted = ParseBool(xReader.GetAttribute(ElementDeletedXmlAttribute));

                if (id.HasValue && order.HasValue)
                {
                    _elementModelCallback.ImportElement(id.Value, name, type, order.Value, expanded, parent, deleted);
                }

                _progressedElementCount++;
                UpdateProgress(progress);
            }
        }

        private void WriteRelations(XmlWriter writer, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(RelationGroupXmlNode);
            foreach (IDsmRelation relation in _relationModelCallback.GetExportedRelations())
            {
                WriteRelation(writer, relation, progress);
            }
            writer.WriteEndElement();
        }

        private void WriteRelation(XmlWriter writer, IDsmRelation relation, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(RelationXmlNode);
            writer.WriteAttributeString(RelationIdXmlAttribute, relation.Id.ToString());
            writer.WriteAttributeString(RelationFromXmlAttribute, relation.ConsumerId.ToString());
            writer.WriteAttributeString(RelationToXmlAttribute, relation.ProviderId.ToString());
            writer.WriteAttributeString(RelationTypeXmlAttribute, relation.Type);
            writer.WriteAttributeString(RelationWeightXmlAttribute, relation.Weight.ToString());
            if (relation.IsDeleted)
            {
                writer.WriteAttributeString(RelationDeletedXmlAttribute, "true");
            }
            writer.WriteEndElement();

            _progressedRelationCount++;
            UpdateProgress(progress);
        }

        private void ReadRelation(XmlReader xReader, IProgress<ProgressInfo> progress)
        {
            if (xReader.Name == RelationXmlNode)
            {
                int? id = ParseInt(xReader.GetAttribute(RelationIdXmlAttribute));
                int? consumer = ParseInt(xReader.GetAttribute(RelationFromXmlAttribute));
                int? provider = ParseInt(xReader.GetAttribute(RelationToXmlAttribute));
                string type = xReader.GetAttribute(RelationTypeXmlAttribute);
                int? weight = ParseInt(xReader.GetAttribute(RelationWeightXmlAttribute));
                bool deleted = ParseBool(xReader.GetAttribute(RelationDeletedXmlAttribute));

                if (id.HasValue &&
                    consumer.HasValue &&
                    provider.HasValue &&
                    weight.HasValue)
                {
                    _relationModelCallback.ImportRelation(id.Value, consumer.Value, provider.Value, type, weight.Value, deleted);
                }

                _progressedRelationCount++;
                UpdateProgress(progress);
            }
        }

        private void WriteActions(XmlWriter writer, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(ActionGroupXmlNode);
            foreach (IDsmAction action in _actionModelCallback.GetExportedActions())
            {
                WriteAction(writer, action, progress);
            }
            writer.WriteEndElement();
        }

        private void WriteAction(XmlWriter writer, IDsmAction action, IProgress<ProgressInfo> progress)
        {
            writer.WriteStartElement(ActionXmlNode);
            writer.WriteAttributeString(ActionIdXmlAttribute, action.Id.ToString());
            writer.WriteAttributeString(ActionTypeXmlAttribute, action.Type);
            writer.WriteStartElement(ActionDataXmlNode);
            foreach (var d in action.Data)
            {
                writer.WriteAttributeString(d.Key, d.Value);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();

            _progressedActionCount++;
            UpdateProgress(progress);
        }

        private void ReadAction(XmlReader xReader, IProgress<ProgressInfo> progress)
        {
            if (xReader.Name == ActionXmlNode)
            {
                int? id = ParseInt(xReader.GetAttribute(ActionIdXmlAttribute));
                string type = xReader.GetAttribute(ActionTypeXmlAttribute);
                Dictionary<string, string> data = new Dictionary<string, string>();

                XmlReader xActionDataReader = xReader.ReadSubtree();
                while (xActionDataReader.Read())
                {
                    if (xActionDataReader.Name == ActionDataXmlNode)
                    {
                        while (xActionDataReader.MoveToNextAttribute())
                        {
                            data[xReader.Name] = xReader.Value;
                        }
                        xActionDataReader.MoveToElement();
                    }
                }

                if (id.HasValue)
                {
                    _actionModelCallback.ImportAction(id.Value, type, data);
                }

                _progressedActionCount++;
                UpdateProgress(progress);
            }
        }

        private void UpdateProgress(IProgress<ProgressInfo> progress)
        {
            if (progress != null)
            {
                int totalItemCount = _totalElementCount + _totalRelationCount + _totalActionCount;
                int progressedItemCount = _progressedElementCount + _progressedRelationCount + _progressedActionCount;

                int currentProgress = 0;
                if (totalItemCount > 0)
                {
                    currentProgress = progressedItemCount * 100 / totalItemCount;
                }

                if (_progress != currentProgress)
                {
                    _progress = currentProgress;

                    ProgressInfo progressInfoInfo = new ProgressInfo
                    {
                        ActionText = _progressActionText,
                        ProgressText = $"Elements {_progressedElementCount}/{_totalElementCount} Relations={_progressedRelationCount}/{_totalRelationCount} Actions={_progressedActionCount}/{_totalActionCount}",
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

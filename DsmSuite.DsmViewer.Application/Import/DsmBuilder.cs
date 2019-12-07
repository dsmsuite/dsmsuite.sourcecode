using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import
{
    public class DsmBuilder : IDsiModelFileCallback
    {
        private readonly IDsmModel _model;
        private readonly Dictionary<int, IDsmElement> _elementsById = new Dictionary<int, IDsmElement>();

        public DsmBuilder(IDsmModel model)
        {
            _model = model;
        }

        public void Build(string dsiFilename, string dsmFilename, bool applyPartitionAlgorithm, bool compressDsmFile)
        {
            _model.Clear();

            DsiModelFile dsiModelFile = new DsiModelFile(dsiFilename, this);
            dsiModelFile.Load(null);
            if (applyPartitionAlgorithm)
            {
                Logger.LogUserMessage("Partitioning full model. Please wait!");
                foreach (IDsmElement element in _model.RootElements)
                {
                    Partition(element);
                }
            }
            _model.AssignElementOrder();
            _model.SaveModel(dsmFilename, compressDsmFile, null);
        }

        private void Partition(IDsmElement element)
        {
            Partitioner partitioner = new Partitioner(element, _model);
            Vector vector = partitioner.Partition();
            _model.ReorderChildren(element, vector);

            foreach (IDsmElement child in element.Children)
            {
                Partition(child);
            }
        }

        public void ImportMetaDataItem(string group, string name, string value)
        {
            _model.AddMetaData(group, name, value);
        }

        public void ImportElement(int id, string fullname, string type, string source)
        {
            IDsmElement parent = null;
            ElementName elementName = new ElementName();
            foreach (string name in new ElementName(fullname).NameParts)
            {
                elementName.AddNamePart(name);

                bool isElementLeaf = (fullname == elementName.FullName);
                string elementType = isElementLeaf ? type : "";

                int? parentId = parent?.Id;
                IDsmElement element = _model.CreateElement(name, elementType, parentId);
                parent = element;

                if (isElementLeaf)
                {
                    _elementsById[id] = element;
                }
            }
        }

        public void ImportRelation(int consumer, int provider, string type, int weight)
        {
            if (_elementsById.ContainsKey(consumer) && _elementsById.ContainsKey(provider))
            {
                _model.AddRelation(consumer, provider, type, weight);
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={consumer} provider={provider}");
            }
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return null;
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string group)
        {
            return null;
        }

        public IEnumerable<IDsiElement> GetElements()
        {
            return null;
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            return null;
        }
    }
}

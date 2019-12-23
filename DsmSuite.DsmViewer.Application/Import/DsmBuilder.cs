using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Model.Persistency;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import
{
    public class DsmBuilder : IMetaDataModelFileCallback, IDsiElementModelFileCallback, IDsiRelationModelFileCallback
    {
        private readonly IDsmModel _model;
        private readonly Dictionary<int, int> _dsiToDsmMapping = new Dictionary<int, int>();

        public DsmBuilder(IDsmModel model)
        {
            _model = model;
        }

        public void Build(string dsiFilename, string dsmFilename, bool applyPartitionAlgorithm, bool compressDsmFile)
        {
            _model.Clear();

            DsiModelFile dsiModelFile = new DsiModelFile(dsiFilename, this, this, this);
            dsiModelFile.Load(null);
            if (applyPartitionAlgorithm)
            {
                Logger.LogUserMessage("Partitioning full model. Please wait!");
                foreach (IDsmElement element in _model.GetRootElements())
                {
                    Partition(element);
                }
            }
            _model.AssignElementOrder();
            _model.SaveModel(dsmFilename, compressDsmFile, null);
        }

        private void Partition(IDsmElement element)
        {
            ISortAlgorithm algorithm =  SortAlgorithmFactory.CreateAlgorithm(_model, element, PartitionSortAlgorithm.AlgorithmName);
            ISortResult sortResult = algorithm.Sort();
            _model.ReorderChildren(element, sortResult);

            foreach (IDsmElement child in element.Children)
            {
                Partition(child);
            }
        }

        public IMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            return _model.AddMetaData(group, name, value);
        }

        public IDsiElement ImportElement(int id, string fullname, string type, string source)
        {
            IDsmElement parent = null;
            ElementName elementName = new ElementName();
            foreach (string name in new ElementName(fullname).NameParts)
            {
                elementName.AddNamePart(name);

                bool isElementLeaf = (fullname == elementName.FullName);
                string elementType = isElementLeaf ? type : "";

                int? parentId = parent?.Id;
                IDsmElement element = _model.AddElement(name, elementType, parentId);
                parent = element;

                if (isElementLeaf)
                {
                    _dsiToDsmMapping[id] = element.Id;
                }
            }
            return null;
        }

        public IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            if (_dsiToDsmMapping.ContainsKey(consumerId) && _dsiToDsmMapping.ContainsKey(providerId))
            {
                _model.AddRelation(_dsiToDsmMapping[consumerId], _dsiToDsmMapping[providerId], type, weight);
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={consumerId} provider={providerId}");
            }
            return null;
        }

        public IEnumerable<string> GetExportedMetaDataGroups()
        {
            return null;
        }

        public IEnumerable<IMetaDataItem> GetExportedMetaDataGroupItems(string group)
        {
            return null;
        }

        public IEnumerable<IDsiElement> GetExportedElements()
        {
            return null;
        }

        public IEnumerable<IDsiRelation> GetExportedRelations()
        {
            return null;
        }
    }
}

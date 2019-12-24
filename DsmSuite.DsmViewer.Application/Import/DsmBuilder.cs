using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;
using System;

namespace DsmSuite.DsmViewer.Application.Import
{
    public class DsmBuilder
    {
        private readonly IDsiDataModel _dsiModel;
        private readonly IDsmModel _dsmModel;
        private readonly Dictionary<int, int> _dsiToDsmMapping = new Dictionary<int, int>();

        public DsmBuilder(IDsiDataModel dsiModel, IDsmModel dsmmodel)
        {
            _dsiModel = dsiModel;
            _dsmModel = dsmmodel;
        }

        public void Create(bool autoPartition)
        {
            _dsmModel.Clear();

            foreach (string groupName in _dsiModel.GetMetaDataGroups())
            {
                foreach(IMetaDataItem metaDatItem in _dsiModel.GetMetaDataGroupItems(groupName))
                {
                    ImportMetaDataItem(groupName, metaDatItem);
                }
            }

            foreach (IDsiElement dsiElement in _dsiModel.GetElements())
            {
                ImportElement(dsiElement);
            }

            foreach (IDsiRelation dsiRelation in _dsiModel.GetRelations())
            {
                ImportRelation(dsiRelation);
            }

            if (autoPartition)
            {
                Logger.LogUserMessage("Partitioning full model. Please wait!");
                foreach (IDsmElement element in _dsmModel.GetRootElements())
                {
                    Partition(element);
                }
            }
            _dsmModel.AssignElementOrder();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        private IMetaDataItem ImportMetaDataItem(string groupName, IMetaDataItem metaDatItem)
        {
            return _dsmModel.AddMetaData(groupName, metaDatItem.Name, metaDatItem.Value);
        }

        private IDsiElement ImportElement(IDsiElement dsiElement)
        {
            IDsmElement parent = null;
            ElementName elementName = new ElementName();
            foreach (string name in new ElementName(dsiElement.Name).NameParts)
            {
                elementName.AddNamePart(name);

                bool isElementLeaf = (dsiElement.Name == elementName.FullName);
                string elementType = isElementLeaf ? dsiElement.Type : "";

                int? parentId = parent?.Id;
                IDsmElement element = _dsmModel.AddElement(name, elementType, parentId);
                parent = element;

                if (isElementLeaf)
                {
                    _dsiToDsmMapping[dsiElement.Id] = element.Id;
                }
            }
            return null;
        }

        private IDsiRelation ImportRelation(IDsiRelation dsiRelation)
        {
            if (_dsiToDsmMapping.ContainsKey(dsiRelation.ConsumerId) && _dsiToDsmMapping.ContainsKey(dsiRelation.ProviderId))
            {
                _dsmModel.AddRelation(_dsiToDsmMapping[dsiRelation.ConsumerId], _dsiToDsmMapping[dsiRelation.ProviderId], dsiRelation.Type, dsiRelation.Weight);
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={dsiRelation.ConsumerId} provider={dsiRelation.ProviderId}");
            }
            return null;
        }

        private void Partition(IDsmElement element)
        {
            ISortAlgorithm algorithm = SortAlgorithmFactory.CreateAlgorithm(_dsmModel, element, PartitionSortAlgorithm.AlgorithmName);
            _dsmModel.ReorderChildren(element, algorithm.Sort());

            foreach (IDsmElement child in element.Children)
            {
                Partition(child);
            }
        }
    }
}

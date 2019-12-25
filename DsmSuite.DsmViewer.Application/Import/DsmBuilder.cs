using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Algorithm;
using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Linq;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Actions.Relation;

namespace DsmSuite.DsmViewer.Application.Import
{
    public class DsmBuilder
    {
        private readonly IDsiDataModel _dsiModel;
        private readonly IDsmModel _dsmModel;
        private readonly IActionManager _actionManager;
        private readonly Dictionary<int, int> _dsiToDsmMapping;

        private enum Mode
        {
            Create,
            Update
        }

        public DsmBuilder(IDsiDataModel dsiModel, IDsmModel dsmmodel, IActionManager actionManager)
        {
            _dsiModel = dsiModel;
            _dsmModel = dsmmodel;
            _actionManager = actionManager;
            _dsiToDsmMapping = new Dictionary<int, int>();
        }

        public void CreateDsm(bool autoPartition)
        {
            _dsmModel.Clear();

            UpdateMetaDataItems(Mode.Create);
            UpdateElements(Mode.Create);
            UpdateRelations(Mode.Create);

            if (autoPartition)
            {
                Partition();
            }

            _dsmModel.AssignElementOrder();
        }

        public void UpdateDsm()
        {

            UpdateMetaDataItems(Mode.Update);
            UpdateElements(Mode.Update);
            UpdateRelations(Mode.Update);

            throw new NotImplementedException();
        }

        private void UpdateMetaDataItems(Mode mode)
        {
            foreach (string groupName in _dsiModel.GetMetaDataGroups())
            {
                foreach (IMetaDataItem metaDatItem in _dsiModel.GetMetaDataGroupItems(groupName))
                {
                    UpdateMetaDataItem(mode, groupName, metaDatItem);
                }
            }
        }

        private IMetaDataItem UpdateMetaDataItem(Mode mode, string groupName, IMetaDataItem metaDatItem)
        {
            return _dsmModel.AddMetaData(groupName, metaDatItem.Name, metaDatItem.Value);
        }

        private void UpdateElements(Mode mode)
        {
            Dictionary<int, IDsmElement> existingDsmElements = new Dictionary<int, IDsmElement>();

            if (mode == Mode.Update)
            {
                existingDsmElements = _dsmModel.GetElements().ToDictionary(x => x.Id, x => x);
            }

            foreach (IDsiElement dsiElement in _dsiModel.GetElements())
            {
                UpdateElement(mode, existingDsmElements, dsiElement);
            }

            if (mode == Mode.Update)
            {
                foreach (IDsmElement dsmElement in existingDsmElements.Values)
                {
                    ElementDeleteAction action = new ElementDeleteAction(_dsmModel, dsmElement);
                    _actionManager.Add(action);
                }
            }
        }

        private IDsiElement UpdateElement(Mode mode, IDictionary<int, IDsmElement> existingDsmElements, IDsiElement dsiElement)
        {
            IDsmElement parent = null;
            ElementName elementName = new ElementName();
            foreach (string name in new ElementName(dsiElement.Name).NameParts)
            {
                elementName.AddNamePart(name);

                bool isElementLeaf = (dsiElement.Name == elementName.FullName);
                string elementType = isElementLeaf ? dsiElement.Type : "";

                IDsmElement element = _dsmModel.GetElementByFullname(elementName.FullName);
                if (element != null)
                {
                    existingDsmElements.Remove(element.Id);
                }
                else
                {
                    if (mode == Mode.Create)
                    {
                        int? parentId = parent?.Id;
                        element = _dsmModel.AddElement(name, elementType, parentId);
                    }
                    else
                    {
                        ElementCreateAction action = new ElementCreateAction(_dsmModel, name, elementType, parent);
                        _actionManager.Add(action);
                        element = action.CreatedElement;
                    }
                }

                parent = element;

                if (isElementLeaf)
                {
                    _dsiToDsmMapping[dsiElement.Id] = element.Id;
                }
            }
            return null;
        }

        private void UpdateRelations(Mode mode)
        {
            Dictionary<int, IDsmRelation> existingDsmRelations = new Dictionary<int, IDsmRelation>();

            if (mode == Mode.Update)
            {
                existingDsmRelations = _dsmModel.GetRelations().ToDictionary(x => x.Id, x => x);
            }

            foreach (IDsiRelation dsiRelation in _dsiModel.GetRelations())
            {
                UpdateRelation(mode, existingDsmRelations, dsiRelation);
            }

            if (mode == Mode.Update)
            {
                foreach (IDsmRelation dsmRelation in existingDsmRelations.Values)
                {
                    RelationDeleteAction action = new RelationDeleteAction(_dsmModel, dsmRelation);
                    _actionManager.Add(action);
                }
            }
        }

        private IDsiRelation UpdateRelation(Mode mode, IDictionary<int, IDsmRelation> existingDsmRelations, IDsiRelation dsiRelation)
        {
            if (_dsiToDsmMapping.ContainsKey(dsiRelation.ConsumerId) && _dsiToDsmMapping.ContainsKey(dsiRelation.ProviderId))
            {
                int dsmConsumerId = _dsiToDsmMapping[dsiRelation.ConsumerId];
                int dsmProviderId = _dsiToDsmMapping[dsiRelation.ProviderId];
                string dsmRelationType = dsiRelation.Type;
                int dsmRelationWeight = dsiRelation.Weight;

                if (mode == Mode.Update)
                {
                    IDsmRelation relation = _dsmModel.FindRelation(dsmConsumerId, dsmProviderId, dsmRelationType);
                    if (relation != null)
                    {
                        existingDsmRelations.Remove(relation.Id);

                        if (relation.Weight != dsmRelationWeight)
                        {
                            RelationChangeWeightAction action = new RelationChangeWeightAction(_dsmModel, relation, dsmRelationWeight);
                            _actionManager.Add(action);
                        }
                    }
                    else
                    {
                        RelationCreateAction action = new RelationCreateAction(_dsmModel, dsmConsumerId, dsmProviderId, dsmRelationType, dsmRelationWeight);
                        _actionManager.Add(action);
                    }
                }
                else
                {
                    _dsmModel.AddRelation(dsmConsumerId, dsmProviderId, dsmRelationType, dsmRelationWeight);
                }
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={dsiRelation.ConsumerId} provider={dsiRelation.ProviderId}");
            }
            return null;
        }

        private void Partition()
        {
            Logger.LogUserMessage("Partitioning full model. Please wait!");
            foreach (IDsmElement element in _dsmModel.GetRootElements())
            {
                Partition(element);
            }
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

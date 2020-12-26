using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Import.Common;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import.Dsi
{
    public class DsiImporter : ImporterBase
    {
        private readonly IDsiModel _dsiModel;
        private readonly IDsmBuilder _importPolicy;
        private readonly bool _autoPartition;
        private readonly Dictionary<int, int> _dsiToDsmMapping;

        public DsiImporter(IDsiModel dsiModel, IDsmModel dsmModel, IDsmBuilder importPolicy, bool autoPartition) : base(dsmModel)
        {
            _dsiModel = dsiModel;
            _importPolicy = importPolicy;
            _autoPartition = autoPartition;
            _dsiToDsmMapping = new Dictionary<int, int>();
        }

        public void Import(IProgress<ProgressInfo> progress)
        {
            ImportMetaDataItems();
            ImportElements(progress);
            ImportRelations(progress);

            if (_autoPartition)
            {
                Partition(progress);
            }

            _importPolicy.FinalizeImport(progress);
        }

        private void ImportMetaDataItems()
        {
            foreach (string groupName in _dsiModel.GetMetaDataGroups())
            {
                foreach (IMetaDataItem metaDatItem in _dsiModel.GetMetaDataGroupItems(groupName))
                {
                    _importPolicy.ImportMetaDataItem(groupName, metaDatItem.Name, metaDatItem.Value);
                }
            }
        }

        private void ImportElements(IProgress<ProgressInfo> progress)
        {
            int totalElements = _dsiModel.GetElements().Count();
            int progressedElements = 0;
            foreach (IDsiElement dsiElement in _dsiModel.GetElements())
            {
                ImportElement(dsiElement);
                progressedElements++;
                UpdateProgress(progress, "Import elements", totalElements, progressedElements);
            }
        }

        private void ImportElement(IDsiElement dsiElement)
        {
            IDsmElement parent = null;
            ElementName elementName = new ElementName();
            foreach (string name in new ElementName(dsiElement.Name).NameParts)
            {
                elementName.AddNamePart(name);

                bool isElementLeaf = (dsiElement.Name == elementName.FullName);
                string elementType = isElementLeaf ? dsiElement.Type : "";

                IDsmElement element = _importPolicy.ImportElement(elementName.FullName, name, elementType, parent, dsiElement.Annotation);
                parent = element;

                if (isElementLeaf)
                {
                    _dsiToDsmMapping[dsiElement.Id] = element.Id;
                }
            }
        }

        private void ImportRelations(IProgress<ProgressInfo> progress)
        {
            int totalRelations = _dsiModel.GetRelations().Count();
            int progressedRelations = 0;
            foreach (IDsiRelation dsiRelation in _dsiModel.GetRelations())
            {
                ImportRelation(dsiRelation);
                progressedRelations++;
                UpdateProgress(progress, "Import relations", totalRelations, progressedRelations);
            }
        }
        
        private void ImportRelation(IDsiRelation dsiRelation)
        {
            if (_dsiToDsmMapping.ContainsKey(dsiRelation.ConsumerId) && _dsiToDsmMapping.ContainsKey(dsiRelation.ProviderId))
            {
                int consumerId = _dsiToDsmMapping[dsiRelation.ConsumerId];
                int providerId = _dsiToDsmMapping[dsiRelation.ProviderId];
                string type = dsiRelation.Type;
                int weight = dsiRelation.Weight;

                if (consumerId != providerId)
                {
                    _importPolicy.ImportRelation(consumerId, providerId, type, weight, dsiRelation.Annotation);
                }
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={dsiRelation.ConsumerId} provider={dsiRelation.ProviderId}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Sorting;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Import
{
    public class DsmBuilder
    {
        private readonly IDsiModel _dsiModel;
        private readonly IDsmModel _dsmModel;
        private readonly IImportPolicy _importPolicy;
        private bool _autoPartition;
        private readonly Dictionary<int, int> _dsiToDsmMapping;
        private int _progress;

        public DsmBuilder(IDsiModel dsiModel, IDsmModel dsmModel, IImportPolicy importPolicy, bool autoPartition)
        {
            _dsiModel = dsiModel;
            _dsmModel = dsmModel;
            _importPolicy = importPolicy;
            _autoPartition = autoPartition;
            _dsiToDsmMapping = new Dictionary<int, int>();
        }

        public void Build(IProgress<ProgressInfo> progress)
        {
            ImportMetaDataItems();
            ImportDsiElements(progress);
            ImportDsiRelations(progress);

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

        private void ImportDsiElements(IProgress<ProgressInfo> progress)
        {
            int totalElements = _dsiModel.GetElements().Count();
            int progressedElements = 0;
            foreach (IDsiElement dsiElement in _dsiModel.GetElements())
            {
                ImportDsiElement(dsiElement);
                progressedElements++;
                UpdateProgress(progress, "Import elements", totalElements, progressedElements);
            }
        }

        private void ImportDsiElement(IDsiElement dsiElement)
        {
            IDsmElement parent = null;
            ElementName elementName = new ElementName();
            foreach (string name in new ElementName(dsiElement.Name).NameParts)
            {
                elementName.AddNamePart(name);

                bool isElementLeaf = (dsiElement.Name == elementName.FullName);
                string elementType = isElementLeaf ? dsiElement.Type : "";

                IDsmElement element = _importPolicy.ImportElement(elementName.FullName, name, elementType, parent);
                parent = element;

                if (isElementLeaf)
                {
                    _dsiToDsmMapping[dsiElement.Id] = element.Id;
                }
            }
        }

        private void ImportDsiRelations(IProgress<ProgressInfo> progress)
        {
            int totalRelations = _dsiModel.GetRelations().Count();
            int progressedRelations = 0;
            foreach (IDsiRelation dsiRelation in _dsiModel.GetRelations())
            {
                ImportDsiRelation(dsiRelation);
                progressedRelations++;
                UpdateProgress(progress, "Import relations", totalRelations, progressedRelations);
            }
        }
        
        private void ImportDsiRelation(IDsiRelation dsiRelation)
        {
            if (_dsiToDsmMapping.ContainsKey(dsiRelation.ConsumerId) && _dsiToDsmMapping.ContainsKey(dsiRelation.ProviderId))
            {
                int consumerId = _dsiToDsmMapping[dsiRelation.ConsumerId];
                int providerId = _dsiToDsmMapping[dsiRelation.ProviderId];
                string type = dsiRelation.Type;
                int weight = dsiRelation.Weight;

                if (consumerId != providerId)
                {
                    _importPolicy.ImportRelation(consumerId, providerId, type, weight);
                }
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={dsiRelation.ConsumerId} provider={dsiRelation.ProviderId}");
            }
        }

        private void Partition(IProgress<ProgressInfo> progress)
        {
            int totalElements = _dsmModel.GetElementCount();
            int progressedElements = 0;
            Partition(progress, _dsmModel.GetRootElement(), totalElements, ref progressedElements);
        }

        private void Partition(IProgress<ProgressInfo> progress, IDsmElement element, int totalElements, ref int progressedElements)
        {
            ISortAlgorithm algorithm = SortAlgorithmFactory.CreateAlgorithm(_dsmModel, element, PartitionSortAlgorithm.AlgorithmName);
            _dsmModel.ReorderChildren(element, algorithm.Sort());
            progressedElements++;
            UpdateProgress(progress, "Partition elements", totalElements, progressedElements);

            foreach (IDsmElement child in element.Children)
            {
                Partition(progress, child, totalElements, ref progressedElements);
            }
        }

        protected void UpdateProgress(IProgress<ProgressInfo> progress, string progressActionText, int totalItemCount, int progressedItemCount)
        {
            if (progress != null)
            {
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
                        ActionText = progressActionText,
                        ProgressText = $"{progressedItemCount}/{totalItemCount}",
                        Progress = _progress
                    };

                    progress.Report(progressInfoInfo);
                }
            }
        }
    }
}

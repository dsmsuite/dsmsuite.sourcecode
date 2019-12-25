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
        private readonly IImportPolicy _importPolicy;
        private readonly Dictionary<int, int> _dsiToDsmMapping;

        public DsmBuilder(IDsiDataModel dsiModel, IImportPolicy importPolicy)
        {
            _dsiModel = dsiModel;
            _importPolicy = importPolicy;
            _dsiToDsmMapping = new Dictionary<int, int>();
        }

        public void Build()
        {
            ImportMetaDataItems();
            ImportDsiElements();
            ImportDsiRelations();

            _importPolicy.FinalizeImport();
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

        private void ImportDsiElements()
        {
            foreach (IDsiElement dsiElement in _dsiModel.GetElements())
            {
                ImportDsiElement(dsiElement);
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

        private void ImportDsiRelations()
        {
            foreach (IDsiRelation dsiRelation in _dsiModel.GetRelations())
            {
                ImportDsiRelation(dsiRelation);
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

                _importPolicy.ImportRelation(consumerId, providerId, type, weight);
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={dsiRelation.ConsumerId} provider={dsiRelation.ProviderId}");
            }
        }
    }
}

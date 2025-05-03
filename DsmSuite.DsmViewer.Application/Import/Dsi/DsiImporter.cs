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
        private readonly IDsmModel _dsmModel;
        private readonly bool _autoPartition;
        private readonly Dictionary<int, int> _dsiToDsmMapping;
        private int _totalItemCount;
        private int _progressedItemCount;

        public DsiImporter(IDsiModel dsiModel, IDsmModel dsmModel,  bool autoPartition) : base(dsmModel)
        {
            _dsiModel = dsiModel;
            _dsmModel = dsmModel;
            _autoPartition = autoPartition;
            _dsiToDsmMapping = new Dictionary<int, int>();

            _totalItemCount = _dsiModel.GetElements().Count() + _dsiModel.GetRelations().Count();
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

            FinalizeImport(progress);
        }

        private void ImportMetaDataItems()
        {
            foreach (string groupName in _dsiModel.GetMetaDataGroups())
            {
                foreach (IMetaDataItem metaDatItem in _dsiModel.GetMetaDataGroupItems(groupName))
                {
                    ImportMetaDataItem(groupName, metaDatItem.Name, metaDatItem.Value);
                }
            }
        }

        private void ImportElements(IProgress<ProgressInfo> progress)
        {
            foreach (IDsiElement dsiElement in _dsiModel.GetElements())
            {
                ImportElement(dsiElement);
                _progressedItemCount++;
                UpdateProgress(progress, "Importing dsi model", _totalItemCount, _progressedItemCount);
            }
        }

        private void ImportElement(IDsiElement dsiElement)
        {
            IDsmElement parent = null;

            char[] trimCharacters = { ' ', '.' };
            string dsiElementName = dsiElement.Name.TrimStart(trimCharacters);
            ElementName elementName = new ElementName();
            foreach (string name in new ElementName(dsiElementName).NameParts)
            {
                elementName.AddNamePart(name);

                bool isElementLeaf = (dsiElementName == elementName.FullName);

                if (isElementLeaf)
                {
                    IDsmElement element = ImportElement(elementName.FullName, name, dsiElement.Type, parent, dsiElement.Properties);
                    parent = element;
                    _dsiToDsmMapping[dsiElement.Id] = element.Id;

                    Logger.LogInfo($"Import leaf element dsiId={dsiElement.Id} dsiName={dsiElement.Name} dsmId={element.Id} dsmName={elementName.FullName}");
                }
                else
                {
                    IDsmElement element = ImportElement(elementName.FullName, name, "", parent, null);
                    parent = element;

                    Logger.LogInfo($"Import non leaf element dsiId={dsiElement.Id} dsiName={dsiElement.Name} dsmName={elementName.FullName}");
                }
            }
        }

        private void ImportRelations(IProgress<ProgressInfo> progress)
        {
            foreach (IDsiRelation dsiRelation in _dsiModel.GetRelations())
            {
                ImportRelation(dsiRelation);
                _progressedItemCount++;
                UpdateProgress(progress, "Importing dsi model", _totalItemCount, _progressedItemCount);
            }
        }

        private void ImportRelation(IDsiRelation dsiRelation)
        {
            Logger.LogInfo($"Import relation consumerId={dsiRelation.ConsumerId} providerId={dsiRelation.ProviderId}");

            if (_dsiToDsmMapping.ContainsKey(dsiRelation.ConsumerId) && _dsiToDsmMapping.ContainsKey(dsiRelation.ProviderId))
            {
                int consumerId = _dsiToDsmMapping[dsiRelation.ConsumerId];
                int providerId = _dsiToDsmMapping[dsiRelation.ProviderId];
                string type = dsiRelation.Type;
                int weight = dsiRelation.Weight;

                if (consumerId != providerId)
                {
                    ImportRelation(consumerId, providerId, type, weight, dsiRelation.Properties);
                }
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={dsiRelation.ConsumerId} provider={dsiRelation.ProviderId}");
            }
        }

        private IMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            return _dsmModel.AddMetaData(group, name, value);
        }

        private IDsmElement ImportElement(string fullname, string name, string type, IDsmElement parent, IDictionary<string, string> properties)
        {
            IDsmElement element = _dsmModel.GetElementByFullname(fullname);
            if (element == null)
            {
                int? parentId = parent?.Id;
                element = _dsmModel.AddElement(name, type, parentId, 0, properties);
            }
            return element;
        }

        private IDsmRelation ImportRelation(int consumerId, int providerId, string type, int weight, IDictionary<string, string> properties)
        {
            IDsmElement consumer = _dsmModel.GetElementById(consumerId);
            IDsmElement provider = _dsmModel.GetElementById(providerId);

            return _dsmModel.AddRelation(consumer, provider, type, weight, properties);
        }
    }
}

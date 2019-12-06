using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Util;
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

        public void BuildModel(string dsiFilename, string dsmFilename, bool overwriteDsmFile, bool compressDsmFile)
        {
            _model.Clear();

            DsiModelFile dsiModelFile = new DsiModelFile(dsiFilename, this);
            dsiModelFile.Load(null);
            _model.AssignElementOrder();
            _model.SaveModel(dsmFilename, compressDsmFile, null);
        }

        public void ImportMetaDataItem(string groupName, string name, string value)
        {
            _model.AddMetaData(groupName, name, value);
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

        public void ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            if (_elementsById.ContainsKey(consumerId) && _elementsById.ContainsKey(providerId))
            {
                IDsmElement consumer = _elementsById[consumerId];
                IDsmElement provider = _elementsById[providerId];
                _model.AddRelation(consumer.Id, provider.Id, type, weight);
            }
            else
            {
                Logger.LogError($"Could not find consumer or provider of relation consumer={consumerId} provider={providerId}");
            }
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return null;
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string groupName)
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

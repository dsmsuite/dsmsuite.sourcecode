using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.Analyzer.Model.Data;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Dependencies;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Builder
{
    public class Builder : IDsiModelFileCallback
    {
        private readonly BuilderSettings _builderSettings;
        private readonly IDsmModel _model;
        private readonly Dictionary<int, IDsmElement> _elementsById = new Dictionary<int, IDsmElement>();

        public Builder(IDsmModel model, BuilderSettings builderSettings)
        {
            _builderSettings = builderSettings;
            _model = model;
        }

        public void BuildModel()
        {
            Logger.LogUserMessage("Build model");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _model.Clear();

            DsiModelFile dsiModelFile = new DsiModelFile(_builderSettings.InputFilename, this);
            dsiModelFile.Load(null);
            _model.AssignElementOrder();
            _model.SaveModel(_builderSettings.OutputFilename, _builderSettings.CompressOutputFile, null);

            Logger.LogResourceUsage();

            stopWatch.Stop();
            Logger.LogUserMessage($" total elapsed time = {stopWatch.Elapsed}");
        }
        
        public void FoundMetaData(string group, string name, string value)
        {
            _model.AddMetaData(group, name, value);
        }

        public void FoundElement(int id, string fullname, string type)
        {

        }

        public void FoundRelation(int consumerId, int providerId, string type, int weight)
        {

        }

        public IDsiMetaDataItem ImportMetaDataItem(string groupName, string name, string value)
        {
            DsiMetaDataItem metatDataItem = new DsiMetaDataItem(name, value);
            _model.AddMetaData(groupName, name, value);
            return metatDataItem;
        }

        public IDsiElement ImportElement(int id, string name, string type, string source)
        {
            IDsmElement parent = null;
            HierarchicalName elementName = new HierarchicalName();
            foreach (string namePart in new HierarchicalName(name).Elements)
            {
                elementName.Add(namePart);

                bool isElementLeaf = (namePart == elementName.FullName);
                string elementType = isElementLeaf ? type : "";

                int? parentId = parent?.Id;
                IDsmElement element = _model.CreateElement(namePart, elementType, parentId);
                parent = element;

                if (isElementLeaf)
                {
                    _elementsById[id] = element;
                }
            }
            return null;
        }

        public IDsiRelation ImportRelation(int consumerId, int providerId, string type, int weight)
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
            return null;
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IDsiMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IDsiElement> GetElements()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            throw new System.NotImplementedException();
        }
    }
}

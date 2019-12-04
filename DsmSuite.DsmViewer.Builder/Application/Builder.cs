using System.Collections.Generic;
using System.Diagnostics;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Builder.Settings;
using DsmSuite.DsmViewer.Model.Core;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Builder.Application
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
        
        public void ImportMetaDataItem(string groupName, string name, string value)
        {
            _model.AddMetaData(groupName, name, value);
        }

        public void ImportElement(int id, string fullname, string type, string source)
        {
            IDsmElement parent = null;
            HierarchicalName elementName = new HierarchicalName();
            foreach (string name in new HierarchicalName(fullname).Elements)
            {
                elementName.Add(name);

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

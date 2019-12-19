using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Analyzer.Model.Persistency;
using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Util;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.Analyzer.Model.Core
{
    public class DsiDataModel : IDsiDataModel, IDsiModelFileCallback
    {
        private readonly MetaDataModel _metaDataModel;
        private readonly DsiElementsDataModel _elementsDataModel;
        private readonly DsiRelationsDataModel _relationsDataModel;

        public DsiDataModel(string processStep, Assembly executingAssembly)
        {
            _metaDataModel = new MetaDataModel(processStep, executingAssembly);
            _elementsDataModel = new DsiElementsDataModel();
            _relationsDataModel = new DsiRelationsDataModel(_elementsDataModel);
        }

        public void Load(string dsiFilename)
        {
            Logger.LogDataModelMessage($"Load data model file={dsiFilename}");

            DsiModelFile modelFile = new DsiModelFile(dsiFilename, this);
            modelFile.Load(null);
        }

        public void Save(string dsiFilename, bool compressFile)
        {
            Logger.LogDataModelMessage($"Save data model file={dsiFilename} compresss={compressFile}");

            foreach (string type in GetElementTypes())
            {
                _metaDataModel.AddMetaDataItemToDefaultGroup($"- '{type}' elements found", $"{GetElementTypeCount(type)}");
            }
            _metaDataModel.AddMetaDataItemToDefaultGroup("Total elements found", $"{TotalElementCount}");

            foreach (string type in GetRelationTypes())
            {
                _metaDataModel.AddMetaDataItemToDefaultGroup($"- '{type}' relations found", $"{GetRelationTypeCount(type)}");
            }
            _metaDataModel.AddMetaDataItemToDefaultGroup("Total relations found", $"{TotalRelationCount}");
            _metaDataModel.AddMetaDataItemToDefaultGroup("Total relations resolved", $"{ResolvedRelationCount} (confidence={ResolvedRelationPercentage:0.000} %)");

            DsiModelFile modelFile = new DsiModelFile(dsiFilename, this);
            modelFile.Save(compressFile, null);
        }

        public void AddMetaData(string name, string value)
        {
            _metaDataModel.AddMetaDataItemToDefaultGroup(name, value);
        }

        public void ImportMetaDataItem(string group, string name, string value)
        {
            _metaDataModel.AddMetaDataItem(group, name, value);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataModel.GetMetaDataGroups();
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string group)
        {
            return _metaDataModel.GetMetaDataGroupItems(group);
        }
        
        public void ImportElement(int id, string name, string type, string source)
        {
            _elementsDataModel.ImportElement(id, name, type, source);
        }
        
        public IDsiElement AddElement(string name, string type, string source)
        {
            return _elementsDataModel.AddElement(name, type, source);
        }
        
        public void RemoveElement(IDsiElement element)
        {
            _elementsDataModel.RemoveElement(element);
        }

        public void RenameElement(IDsiElement element, string newName)
        {
            _elementsDataModel.RenameElement(element, newName);
        }

        public IDsiElement FindElementById(int id)
        {
            return _elementsDataModel.FindElementById(id);
        }

        public IDsiElement FindElementByName(string name)
        {
            return _elementsDataModel.FindElementByName(name);
        }
        
        public IEnumerable<IDsiElement> GetElements()
        {
            return _elementsDataModel.GetElements();
        }

        public ICollection<string> GetElementTypes()
        {
            return _elementsDataModel.GetElementTypes();
        }

        public int GetElementTypeCount(string type)
        {
            return _elementsDataModel.GetElementTypeCount(type);
        }
        
        public int TotalElementCount => _elementsDataModel.TotalElementCount;
        
        public void ImportRelation(int consumerId, int providerId, string type, int weight)
        {
            _relationsDataModel.ImportRelation(consumerId, providerId, type, weight);
        }

        public IDsiRelation AddRelation(string consumerName, string providerName, string type, int weight, string context)
        {
            return _relationsDataModel.AddRelation(consumerName, providerName, type, weight, context);
        }

        public void SkipRelation(string consumerName, string providerName, string type, string context)
        {
            _relationsDataModel.SkipRelation(consumerName, providerName, type, context);
        }

        public ICollection<string> GetRelationTypes()
        {
            return _relationsDataModel.GetRelationTypes();
        }

        public int GetRelationTypeCount(string type)
        {
            return _relationsDataModel.GetRelationTypeCount(type);
        }
        
        public ICollection<IDsiRelation> GetRelationsOfConsumer(int consumerId)
        {
            return _relationsDataModel.GetRelationsOfConsumer(consumerId);
        }

        public IEnumerable<IDsiRelation> GetRelations()
        {
            return _relationsDataModel.GetRelations();
        }

        public bool DoesRelationExist(int consumerId, int providerId)
        {
            return _relationsDataModel.DoesRelationExist(consumerId, providerId);
        }

        public int TotalRelationCount => _relationsDataModel.TotalRelationCount;

        public int ResolvedRelationCount => _relationsDataModel.ResolvedRelationCount;

        public double ResolvedRelationPercentage => _relationsDataModel.ResolvedRelationPercentage;

        public void Cleanup()
        {
            //_relationsDataModel.RemoveRelationsForRemovedElements();
        }
    }
}

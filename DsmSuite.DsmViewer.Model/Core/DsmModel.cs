using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Data;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmModel : IDsmModel, IDsmModelFileCallback
    {
        private readonly MetaDataModel _metaDataModel;
        private readonly ElementsDataModel _elementsDataModel;
        private readonly RelationsDataModel _relationsDataModel;
        private bool _isModified;

        public event EventHandler<bool> Modified;

        public DsmModel(string processStep, Assembly executingAssembly)
        {
            _metaDataModel = new MetaDataModel(processStep, executingAssembly);
            _elementsDataModel = new ElementsDataModel();
            _relationsDataModel = new RelationsDataModel(_elementsDataModel);
        }

        public void LoadModel(string dsmFilename, IProgress<DsmProgressInfo> progress)
        {
            Logger.LogDataModelMessage($"Load data model file={dsmFilename}");

            Clear();
            DsmModelFile dsmModelFile = new DsmModelFile(dsmFilename, this);
            dsmModelFile.Load(progress);
            IsCompressed = dsmModelFile.IsCompressedFile();
            ModelFilename = dsmFilename;
        }

        public void SaveModel(string dsmFilename, bool compressFile, IProgress<DsmProgressInfo> progress)
        {
            Logger.LogDataModelMessage($"Save data model file={dsmFilename} compresss={compressFile}");

            _metaDataModel.AddMetaData("Total elements found", $"{ElementCount}");

            DsmModelFile dsmModelFile = new DsmModelFile(dsmFilename, this);
            dsmModelFile.Save(compressFile, progress);
            IsModified = false;
            ModelFilename = dsmFilename;
        }

        public string ModelFilename { get; private set; }

        public bool IsModified
        {
            get
            {
                return _isModified;
            }
            private set
            {
                _isModified = value;
                Modified?.Invoke(this, _isModified);
            }
        }

        public bool IsCompressed { get; private set; }

        public void Clear()
        {
            _metaDataModel.Clear();
            _elementsDataModel.Clear();
            _relationsDataModel.Clear();
        }

        public IDsmMetaDataItem AddMetaData(string name, string value)
        {
            return _metaDataModel.AddMetaData(name, value);
        }

        public IDsmMetaDataItem AddMetaData(string group, string name, string value)
        {
            return _metaDataModel.AddMetaData(group, name, value);
        }

        public IDsmMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            return _metaDataModel.ImportMetaDataItem(group, name, value);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataModel.GetMetaDataGroups();
        }

        public IEnumerable<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return _metaDataModel.GetMetaDataGroupItems(groupName);
        }

        public IEnumerable<IDsmElement> RootElements => _elementsDataModel.RootElements;

        public IDsmElement ImportElement(int id, string name, string type, int order, bool expanded, int? parentId)
        {
            return _elementsDataModel.ImportElement(id, name, type, order, expanded, parentId);
        }

        public IDsmElement AddElement(string name, string type, int? parentId)
        {
            return _elementsDataModel.AddElement(name, type, parentId);
        }

        public void EditElement(IDsmElement element, string name, string type)
        {
            _elementsDataModel.EditElement(element, name, type);
        }

        public void ChangeParent(IDsmElement element, IDsmElement parent)
        {
            _elementsDataModel.ChangeParent(element, parent);
        }

        public void RemoveElement(int id)
        {
            _elementsDataModel.RemoveElement(id);
        }

        public void UnremoveElement(int id)
        {
            _elementsDataModel.UnremoveElement(id);
        }


        public IEnumerable<IDsmElement> GetRootElements()
        {
            return _elementsDataModel.GetRootElements();
        }

        public int GetElementCount()
        {
            return _elementsDataModel.GetElementCount();
        }

        public void AssignElementOrder()
        {
            _elementsDataModel.AssignElementOrder();
        }

        public int ElementCount => _elementsDataModel.ElementCount;

        public IDsmElement GetElementById(int id)
        {
            return _elementsDataModel.GetElementById(id);
        }

        public IDsmElement GetElementByFullname(string fullname)
        {
            return _elementsDataModel.GetElementByFullname(fullname);
        }

        public IEnumerable<IDsmElement> SearchElements(string text)
        {
            return _elementsDataModel.SearchElements(text);
        }

        public IDsmRelation ImportRelation(int relationId, int consumerId, int providerId, string type, int weight)
        {
            return _relationsDataModel.ImportRelation(relationId, consumerId, providerId, type, weight);
        }

        public IDsmRelation AddRelation(int consumerId, int providerId, string type, int weight)
        {
            return _relationsDataModel.AddRelation(consumerId, providerId, type, weight);
        }

        public void EditRelation(IDsmRelation relation, string type, int weight)
        {
            _relationsDataModel.EditRelation(relation, type, weight);
        }

        public void RemoveRelation(int relationId)
        {
            _relationsDataModel.RemoveRelation(relationId);
        }

        public void UnremoveRelation(int relationId)
        {
            _relationsDataModel.UnremoveRelation(relationId);
        }

        public int GetDependencyWeight(int consumerId, int providerId)
        {
            return _relationsDataModel.GetDependencyWeight(consumerId, providerId);
        }

        public bool IsCyclicDependency(int consumerId, int providerId)
        {
            return _relationsDataModel.IsCyclicDependency(consumerId, providerId);
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _relationsDataModel.FindRelations(consumer, provider);
        }

        public IEnumerable<IDsmRelation> FindProviderRelations(IDsmElement element)
        {
            return _relationsDataModel.FindProviderRelations(element);
        }

        public IEnumerable<IDsmRelation> FindConsumerRelations(IDsmElement element)
        {
            return _relationsDataModel.FindConsumerRelations(element);
        }

        public IEnumerable<IDsmRelation> GetRelations()
        {
            return _relationsDataModel.GetRelations();
        }

        public int GetRelationCount()
        {
            return _relationsDataModel.GetRelationCount();
        }

        public void ReorderChildren(IDsmElement element, IVector permutationVector)
        {
            _elementsDataModel.ReorderChildren(element, permutationVector);
        }

        public bool Swap(IDsmElement element1, IDsmElement element2)
        {
            return _elementsDataModel.Swap(element1, element2);
        }

        public IDsmElement NextSibling(IDsmElement element)
        {
            return _elementsDataModel.NextSibling(element);
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            return _elementsDataModel.PreviousSibling(element);
        }
    }
}

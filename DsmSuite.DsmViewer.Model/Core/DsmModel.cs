using System;
using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;
using DsmSuite.Analyzer.Model.Core;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmModel : IDsmModel, IDsmModelFileCallback
    {
        private readonly MetaDataModel _metaDataModel;
        private readonly DsmElementsDataModel _elementsDataModel;
        private readonly DsmRelationsDataModel _relationsDataModel;
        private readonly List<IDsmAction> _actions;

        public DsmModel(string processStep, Assembly executingAssembly)
        {
            _metaDataModel = new MetaDataModel(processStep, executingAssembly);
            _elementsDataModel = new DsmElementsDataModel();
            _relationsDataModel = new DsmRelationsDataModel(_elementsDataModel);
            _actions = new List<IDsmAction>();
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

            _metaDataModel.AddMetaDataItemToDefaultGroup("Total elements found", $"{ElementCount}");

            DsmModelFile dsmModelFile = new DsmModelFile(dsmFilename, this);
            dsmModelFile.Save(compressFile, progress);
            ModelFilename = dsmFilename;
        }

        public string ModelFilename { get; private set; }

        public bool IsCompressed { get; private set; }

        public void Clear()
        {
            _metaDataModel.Clear();
            _elementsDataModel.Clear();
            _relationsDataModel.Clear();
        }

        public IMetaDataItem AddMetaData(string name, string value)
        {
            return _metaDataModel.AddMetaDataItemToDefaultGroup(name, value);
        }

        public IMetaDataItem AddMetaData(string group, string name, string value)
        {
            return _metaDataModel.AddMetaDataItem(group, name, value);
        }

        public IMetaDataItem ImportMetaDataItem(string group, string name, string value)
        {
            return _metaDataModel.AddMetaDataItem(group, name, value);
        }

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataModel.GetMetaDataGroups();
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return _metaDataModel.GetMetaDataGroupItems(groupName);
        }

        public IDsmAction ImportAction(int id, string type, IDictionary<string, string> data)
        {
            IDsmAction action = new DsmAction(id, type, data);
            _actions.Add(action);
            return action;
        }

        public IDsmAction AddAction(int id, string type, IDictionary<string, string> data)
        {
            IDsmAction action = new DsmAction(id, type, data);
            _actions.Add(action);
            return action;
        }

        public void ClearActions()
        {
            _actions.Clear();
        }

        public IEnumerable<IDsmAction> GetActions()
        {
            return _actions;
        }

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
            _elementsDataModel.ChangeElementParent(element, parent);
        }

        public void RemoveElement(IDsmElement element)
        {
            _elementsDataModel.RemoveElement(element);
        }

        public void UnremoveElement(IDsmElement element)
        {
            _elementsDataModel.UnremoveElement(element);
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

        public int ElementCount => _elementsDataModel.TotalElementCount;

        public IDsmElement GetElementById(int id)
        {
            return _elementsDataModel.FindElementById(id);
        }

        public IDsmElement GetElementByFullname(string fullname)
        {
            return _elementsDataModel.FindElementByFullname(fullname);
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

        public void RemoveRelation(IDsmRelation relation)
        {
            _relationsDataModel.RemoveRelation(relation);
        }

        public void UnremoveRelation(IDsmRelation relation)
        {
            _relationsDataModel.UnremoveRelation(relation);
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
            return _relationsDataModel.FindRelationsWhereElementHasProviderRole(element);
        }

        public IEnumerable<IDsmRelation> FindConsumerRelations(IDsmElement element)
        {
            return _relationsDataModel.FindRelationsWhereElementHasConsumerRole(element);
        }

        public IEnumerable<IDsmRelation> GetRelations()
        {
            return _relationsDataModel.GetRelations();
        }

        public int GetRelationCount()
        {
            return _relationsDataModel.GetRelationCount();
        }

        public void ReorderChildren(IDsmElement element, IElementSequence sequence)
        {
            _elementsDataModel.ReorderChildren(element, sequence);
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

        public int GetActionCount()
        {
            return _actions.Count;
        }
    }
}

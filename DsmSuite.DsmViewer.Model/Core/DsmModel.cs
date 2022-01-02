using System;
using System.Collections.Generic;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Model.Persistency;
using DsmSuite.Common.Model.Core;
using DsmSuite.Common.Model.Interface;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmModel : IDsmModel
    {
        private readonly MetaDataModel _metaDataModel;
        private readonly DsmElementModel _elementsDataModel;
        private readonly DsmRelationModel _relationsDataModel;
        private readonly DsmActionModel _actionsDataModel;

        public DsmModel(string processStep, Assembly executingAssembly)
        {
            _metaDataModel = new MetaDataModel(processStep, executingAssembly);

            _relationsDataModel = new DsmRelationModel();
            _elementsDataModel = new DsmElementModel(_relationsDataModel);
            _actionsDataModel = new DsmActionModel();
        }

        public void LoadModel(string dsmFilename, IProgress<ProgressInfo> progress)
        {
            Logger.LogDataModelMessage($"Load data model file={dsmFilename}");

            Clear();
            DsmModelFile dsmModelFile = new DsmModelFile(dsmFilename, _metaDataModel, _elementsDataModel, _relationsDataModel, _actionsDataModel);
            dsmModelFile.Load(progress);
            IsCompressed = dsmModelFile.IsCompressedFile();
            ModelFilename = dsmFilename;
        }

        public void SaveModel(string dsmFilename, bool compressFile, IProgress<ProgressInfo> progress)
        {
            Logger.LogDataModelMessage($"Save data model file={dsmFilename} compress={compressFile}");

            _metaDataModel.AddMetaDataItemToDefaultGroup("Total elements found", $"{GetExportedElementCount()}"); 

            DsmModelFile dsmModelFile = new DsmModelFile(dsmFilename, _metaDataModel, _elementsDataModel, _relationsDataModel, _actionsDataModel);
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

        public IEnumerable<string> GetMetaDataGroups()
        {
            return _metaDataModel.GetExportedMetaDataGroups();
        }

        public IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName)
        {
            return _metaDataModel.GetExportedMetaDataGroupItems(groupName);
        }

        public IDsmAction AddAction(string type, IReadOnlyDictionary<string, string> data)
        {
            return _actionsDataModel.AddAction(type, data);
        }

        public void ClearActions()
        {
            _actionsDataModel.Clear();
        }

        public IEnumerable<IDsmAction> GetActions()
        {
            return _actionsDataModel.GetExportedActions();
        }

        public IDsmElement AddElement(string name, string type, int? parentId, IDictionary<string, string> properties)
        {
            return _elementsDataModel.AddElement(name, type, parentId, properties);
        }

        public void ChangeElementName(IDsmElement element, string name)
        {
            _elementsDataModel.ChangeElementName(element, name);
        }

        public void ChangeElementType(IDsmElement element, string type)
        {
            _elementsDataModel.ChangeElementType(element, type);
        }

        public bool IsChangeElementParentAllowed(IDsmElement element, IDsmElement parent)
        {
            return _elementsDataModel.IsChangeElementParentAllowed(element, parent);
        }

        public void ChangeElementParent(IDsmElement element, IDsmElement parent, int index)
        {
            _elementsDataModel.ChangeElementParent(element, parent, index);
        }

        public void RemoveElement(int elementId)
        {
            _elementsDataModel.RemoveElement(elementId);
        }

        public void UnremoveElement(int elementId)
        {
            _elementsDataModel.UnremoveElement(elementId);
        }

        public IEnumerable<IDsmElement> GetElements()
        {
            return _elementsDataModel.GetElements();
        }

        public int GetElementCount()
        {
            return _elementsDataModel.GetElementCount();
        }

        public IDsmElement GetRootElement()
        {
            return _elementsDataModel.GetRootElement();
        }

        public int GetExportedElementCount()
        {
            return _elementsDataModel.GetExportedElementCount();
        }

        public void AssignElementOrder()
        {
            _elementsDataModel.AssignElementOrder();
        }

        public IDsmElement GetElementById(int id)
        {
            return _elementsDataModel.FindElementById(id);
        }

        public IDsmElement GetElementByFullname(string fullname)
        {
            return _elementsDataModel.FindElementByFullname(fullname);
        }

        public IList<IDsmElement> SearchElements(string searchText, bool caseSensitive, string elementTypeFilter)
        {
            return _elementsDataModel.SearchElements(searchText, caseSensitive, elementTypeFilter);
        }

        public IDsmElement GetDeletedElementById(int id)
        {
            return _elementsDataModel.GetDeletedElementById(id);
        }

        public IDsmRelation AddRelation(IDsmElement consumer, IDsmElement provider, string type, int weight, IDictionary<string, string> properties)
        {
            return _relationsDataModel.AddRelation(consumer, provider, type, weight, properties);
        }

        public void ChangeRelationType(IDsmRelation relation, string type)
        {
            _relationsDataModel.ChangeRelationType(relation, type);
        }

        public void ChangeRelationWeight(IDsmRelation relation, int weight)
        {
            _relationsDataModel.ChangeRelationWeight(relation, weight);
        }

        public void RemoveRelation(int relationId)
        {
            _relationsDataModel.RemoveRelation(relationId);
        }

        public void UnremoveRelation(int relationId)
        {
            _relationsDataModel.UnremoveRelation(relationId);
        }

        public int GetDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _relationsDataModel.GetDependencyWeight(consumer, provider);
        }

        public int GetDirectDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _relationsDataModel.GetDirectDependencyWeight(consumer, provider);
        }

        public CycleType IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _relationsDataModel.IsCyclicDependency(consumer, provider);
        }

        public IDsmRelation GetRelationById(int relationId)
        {
            return _relationsDataModel.GetRelationById(relationId);
        }

        public IDsmRelation GetDeletedRelationById(int relationId)
        {
            return _relationsDataModel.GetDeletedRelationById(relationId);
        }

        public IEnumerable<IDsmRelation> GetRelations()
        {
            return _relationsDataModel.GetRelations();
        }

        public int GetRelationCount()
        {
            return _relationsDataModel.GetRelationCount();
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _relationsDataModel.FindRelations(consumer, provider);
        }

        public int GetRelationCount(IDsmElement consumer, IDsmElement provider)
        {
            return _relationsDataModel.GetRelationCount(consumer, provider);
        }

        public IDsmRelation FindRelation(IDsmElement consumer, IDsmElement provider, string type)
        {
            return _relationsDataModel.FindRelation(consumer, provider, type);
        }

        public IEnumerable<IDsmRelation> FindIngoingRelations(IDsmElement element)
        {
            return _relationsDataModel.FindIngoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindOutgoingRelations(IDsmElement element)
        {
            return _relationsDataModel.FindOutgoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindInternalRelations(IDsmElement element)
        {
            return _relationsDataModel.FindInternalRelations(element);
        }

        public IEnumerable<IDsmRelation> FindExternalRelations(IDsmElement element)
        {
            return _relationsDataModel.FindExternalRelations(element);
        }

        public int GetHierarchicalCycleCount(IDsmElement element)
        {
            return _relationsDataModel.GetHierarchicalCycleCount(element);
        }

        public int GetSystemCycleCount(IDsmElement element)
        {
            return _relationsDataModel.GetSystemCycleCount(element);
        }

        public void ReorderChildren(IDsmElement element, ISortResult sortResult)
        {
            _elementsDataModel.ReorderChildren(element, sortResult);
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
            return _actionsDataModel.GetExportedActionCount();
        }
    }
}

using DsmSuite.Common.Model.Interface;
using DsmSuite.Common.Util;
using System;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmModel
    {
        string ModelFilename { get; }
        bool IsCompressed { get; }

        void Clear();
        void LoadModel(string dsmFilename, IProgress<FileAccessProgressInfo> progress);
        void SaveModel(string dsmFilename, bool compressFile, IProgress<FileAccessProgressInfo> progress);

        IMetaDataItem AddMetaData(string group, string name, string value);
        IMetaDataItem AddMetaData(string itemName, string itemValue);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName);

        IDsmAction AddAction(string type, IReadOnlyDictionary<string, string> data);
        void ClearActions();
        IEnumerable<IDsmAction> GetActions();

        IDsmElement AddElement(string name, string type, int? parentId);
        void RemoveElement(int elementId);
        void UnremoveElement(int elementId);
        void ChangeElementParent(IDsmElement element, IDsmElement parent);
        void ReorderChildren(IDsmElement element, ISortResult sortResult);
        IDsmElement NextSibling(IDsmElement element);
        IDsmElement PreviousSibling(IDsmElement element);
        bool Swap(IDsmElement first, IDsmElement second);
        IEnumerable<IDsmElement> GetRootElements();
        IEnumerable<IDsmElement> GetElements();

        IDsmElement GetElementById(int id);
        IDsmElement GetElementByFullname(string fullname);
        IEnumerable<IDsmElement> SearchElements(string text);

        IDsmElement GetDeletedElementById(int id);

        void AssignElementOrder();
        void ChangeElementName(IDsmElement element, string name);
        void ChangeElementType(IDsmElement element, string type);
        IDsmRelation AddRelation(int consumerId, int providerId, string type, int weight);
        void ChangeRelationType(IDsmRelation relation, string type);
        void ChangeRelationWeight(IDsmRelation relation, int weight);
        void RemoveRelation(int relationId);
        void UnremoveRelation(int relationId);
        int GetDependencyWeight(int consumerId, int providerId);
        bool IsCyclicDependency(int consumerId, int providerId);
        IDsmRelation GetRelationById(int relationId);
        IDsmRelation GetDeletedRelationById(int relationId);
        IEnumerable<IDsmRelation> GetRelations();
        IDsmRelation FindRelation(int consumerId, int providerId, string type);
        IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmRelation> FindProviderRelations(IDsmElement element);
        IEnumerable<IDsmRelation> FindConsumerRelations(IDsmElement element);
    }
}

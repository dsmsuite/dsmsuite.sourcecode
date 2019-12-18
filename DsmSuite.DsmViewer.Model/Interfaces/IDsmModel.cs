using DsmSuite.Common.Model.Interface;
using System;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmModel
    {
        string ModelFilename { get; }
        bool IsCompressed { get; }

        void Clear();
        void LoadModel(string dsmFilename, IProgress<DsmProgressInfo> progress);
        void SaveModel(string dsmFilename, bool compressFile, IProgress<DsmProgressInfo> progress);

        IMetaDataItem AddMetaData(string group, string name, string value);
        IMetaDataItem AddMetaData(string itemName, string itemValue);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IMetaDataItem> GetMetaDataGroupItems(string groupName);

        IDsmAction AddAction(int id, string type, IDictionary<string, string> data);
        void ClearActions();
        IEnumerable<IDsmAction> GetActions();

        IDsmElement AddElement(string name, string type, int? parentId);
        void RemoveElement(int elementId);
        void UnremoveElement(int elementId);
        int ElementCount { get; }
        void ChangeParent(IDsmElement element, IDsmElement parent);
        void ReorderChildren(IDsmElement element, IElementSequence sequence);
        IDsmElement NextSibling(IDsmElement element);
        IDsmElement PreviousSibling(IDsmElement element);
        bool Swap(IDsmElement first, IDsmElement second);
        IEnumerable<IDsmElement> GetRootElements();

        IDsmElement GetElementById(int id);
        IDsmElement GetElementByFullname(string fullname);
        IEnumerable<IDsmElement> SearchElements(string text);

        void AssignElementOrder();
        void EditElement(IDsmElement element, string name, string type);
        IDsmRelation AddRelation(int consumerId, int providerId, string type, int weight);
        void EditRelation(IDsmRelation relation, string type, int weight);
        void RemoveRelation(int relationId);
        void UnremoveRelation(int relationId);
        int GetDependencyWeight(int consumerId, int providerId);
        bool IsCyclicDependency(int consumerId, int providerId);

        IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmRelation> FindProviderRelations(IDsmElement element);
        IEnumerable<IDsmRelation> FindConsumerRelations(IDsmElement element);
    }
}

using System;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Interfaces
{
    public interface IDsmModel
    {
        event EventHandler<bool> Modified;
        string ModelFilename { get; }
        bool IsModified { get; }
        bool IsCompressed { get; }

        void Clear();
        void LoadModel(string dsmFilename, IProgress<DsmProgressInfo> progress);
        void SaveModel(string dsmFilename, bool compressFile, IProgress<DsmProgressInfo> progress);

        IDsmMetaDataItem AddMetaData(string group, string name, string value);
        IDsmMetaDataItem AddMetaData(string itemName, string itemValue);
        IEnumerable<string> GetMetaDataGroups();
        IEnumerable<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName);

        IDsmElement AddElement(string name, string type, int? parentId);
        void RemoveElement(int id);
        void UnremoveElement(int id);
        int ElementCount { get; }
        void ChangeParent(IDsmElement element, IDsmElement parent);
        void ReorderChildren(IDsmElement element, IVector permutationVector);
        IDsmElement NextSibling(IDsmElement element);
        IDsmElement PreviousSibling(IDsmElement element);
        bool Swap(IDsmElement fisrt, IDsmElement second);
        IEnumerable<IDsmElement> RootElements { get; }

        IDsmElement GetElementById(int id);
        IDsmElement GetElementByFullname(string fullname);
        IEnumerable<IDsmElement> SearchElements(string text);

        void AssignElementOrder();
        void EditElement(IDsmElement relation, string name, string type);
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

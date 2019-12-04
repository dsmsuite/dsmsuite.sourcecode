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
        IList<string> GetMetaDataGroups();
        IList<IDsmMetaDataItem> GetMetaDataGroupItems(string groupName);

        IDsmElement CreateElement(string name, string type, int? parentId);
        void AddRelation(int consumerId, int providerId, string type, int weight);
        void RemoveRelation(IDsmRelation relation);
        void RemoveRelation(int consumerId, int providerId, string type, int weight);
        void UnremoveRelation(int consumerId, int providerId, string type, int weight);
        int ElementCount { get; }

        void AssignElementOrder();
        IList<IDsmElement> RootElements { get; }

        IDsmElement GetElementById(int id);
        IDsmElement GetElementByFullname(string fullname);
        IEnumerable<IDsmElement> GetElementsWithFullnameContainingText(string text);

        void RemoveElement(int id);
        void RestoreElement(int id);

        int GetDependencyWeight(int consumerId, int providerId);
        bool IsCyclicDependency(int consumerId, int providerId);

        IList<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider);
        IList<IDsmRelation> FindProviderRelations(IDsmElement element);
        IList<IDsmRelation> FindConsumerRelations(IDsmElement element);

        IList<IDsmResolvedRelation> ResolveRelations(IList<IDsmRelation> relations);

        void ReorderChildren(IDsmElement element, Vector permutationVector);
        IDsmElement NextSibling(IDsmElement element);
        IDsmElement PreviousSibling(IDsmElement element);
        bool Swap(IDsmElement fisrt, IDsmElement second);
    }
}

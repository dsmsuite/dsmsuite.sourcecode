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
        void LoadModel(string dsmFilename, IProgress<ProgressInfo> progress);
        void SaveModel(string dsmFilename, bool compressFile, IProgress<ProgressInfo> progress);

        IDsmElement CreateElement(string name, string type, int? parentId);
        void AddRelation(int consumerId, int providerId, string type, int weight);
        void RemoveRelation(int consumerId, int providerId, string type, int weight);
        void UnremoveRelation(int consumerId, int providerId, string type, int weight);
        int ElementCount { get; }
        void AddMetaData(string group, string name, string value);
        void AssignElementOrder();
        IList<IDsmElement> RootElements { get; }

        IDsmElement GetElementById(int id);
        IDsmElement GetElementByFullname(string fullname);
        IEnumerable<IDsmElement> GetElementsWithFullnameContainingText(string text);

        void RemoveElement(int id);
        void RestoreElement(int id);

        int GetDependencyWeight(IDsmElement consumer, IDsmElement provider);
        bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider);

        IList<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider);
        IList<IDsmRelation> FindProviderRelations(IDsmElement element);
        IList<IDsmRelation> FindConsumerRelations(IDsmElement element);

        IList<IDsmElement> FindProviders(IDsmElement element);
        IList<IDsmElement> FindConsumers(IDsmElement element);

        void Partition(IDsmElement element);
        IDsmElement NextSibling(IDsmElement element);
        IDsmElement PreviousSibling(IDsmElement element);
        bool Swap(IDsmElement fisrt, IDsmElement second);

        IList<string> GetGroups();
        IList<string> GetNames(string group);
        string GetValue(string group, string name);
    }
}

using System;
using System.Collections.Generic;
using DsmSuite.DsmViewer.Util;

namespace DsmSuite.DsmViewer.Model
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

        IElement CreateElement(string name, string type, int? parentId);
        void AddRelation(int consumerId, int providerId, string type, int weight);
        void RemoveRelation(int consumerId, int providerId, string type, int weight);
        void UnremoveRelation(int consumerId, int providerId, string type, int weight);
        int ElementCount { get; }
        void AddMetaData(string group, string name, string value);
        void AssignElementOrder();
        IList<IElement> RootElements { get; }

        IElement GetElementById(int id);
        IElement GetElementByFullname(string fullname);

        void RemoveElement(int id);
        void RestoreElement(int id);

        int GetDependencyWeight(IElement consumer, IElement provider);
        bool IsCyclicDependency(IElement consumer, IElement provider);

        IList<IRelation> FindRelations(IElement consumer, IElement provider);
        IList<IRelation> FindProviderRelations(IElement element);
        IList<IRelation> FindConsumerRelations(IElement element);

        IList<IElement> FindProviders(IElement element);
        IList<IElement> FindConsumers(IElement element);

        void Partition(IElement element);
        IElement NextSibling(IElement element);
        IElement PreviousSibling(IElement element);
        bool Swap(IElement fisrt, IElement second);

        IList<string> GetGroups();
        IList<string> GetNames(string group);
        string GetValue(string group, string name);
    }
}

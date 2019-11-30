using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model;

namespace DsmSuite.DsmViewer.Application
{
    public interface IDsmApplication
    {
        IDsmModel Model { get; }

        Task ImportModel(string dsiFilename, string dsmFilename, bool overwrite, Progress<ProgressInfo> progress);
        Task OpenModel(string dsmFilename, Progress<ProgressInfo> progress);
        Task SaveModel(string dsmFilename, Progress<ProgressInfo> progress);

        IList<IElement> RootElements { get; }
        IEnumerable<IElement> GetElementProvidedElements(IElement element);
        IEnumerable<IElement> GetElementProviders(IElement element);
        IEnumerable<IRelation> FindRelations(IElement consumer, IElement provider);
        IEnumerable<IElement> GetRelationProviders(IElement consumer, IElement provider);
        IEnumerable<IElement> GetElementConsumers(IElement element);
        IEnumerable<IElement> GetRelationConsumers(IElement consumer, IElement provider);

        bool IsFirstChild(IElement element);
        bool IsLastChild(IElement element);
        bool HasChildren(IElement element);
        void Sort(IElement element, string algorithm);
        IEnumerable<string> GetSupportedSortAlgorithms();

        int GetDependencyWeight(IElement consumer, IElement provider);
        bool IsCyclicDependency(IElement consumer, IElement provider);

        IEnumerable<IElement> SearchExecute(string text);
    }
}

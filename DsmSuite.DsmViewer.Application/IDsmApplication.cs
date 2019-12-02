using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application
{
    public interface IDsmApplication
    {
        IDsmModel Model { get; }

        Task ImportModel(string dsiFilename, string dsmFilename, bool overwrite, Progress<ProgressInfo> progress);
        Task OpenModel(string dsmFilename, Progress<ProgressInfo> progress);
        Task SaveModel(string dsmFilename, Progress<ProgressInfo> progress);

        IList<IDsmElement> RootElements { get; }
        IEnumerable<IDsmElement> GetElementProvidedElements(IDsmElement element);
        IEnumerable<IDsmElement> GetElementProviders(IDsmElement element);
        IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmElement> GetElementConsumers(IDsmElement element);
        IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider);

        bool IsFirstChild(IDsmElement element);
        bool IsLastChild(IDsmElement element);
        bool HasChildren(IDsmElement element);
        void Sort(IDsmElement element, string algorithm);
        IEnumerable<string> GetSupportedSortAlgorithms();

        int GetDependencyWeight(IDsmElement consumer, IDsmElement provider);
        bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider);

        IEnumerable<IDsmElement> SearchExecute(string text);
    }
}

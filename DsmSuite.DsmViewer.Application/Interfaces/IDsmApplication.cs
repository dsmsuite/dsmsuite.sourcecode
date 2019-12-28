using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public interface IDsmApplication
    {
        event EventHandler<bool> Modified;
        event EventHandler ActionPerformed;

        bool ShowCycles { get; set; }
        void ImportModel(string dsiFilename, string dsmFilename, bool applyPartitionAlgorithm, bool overwriteDsmFile, bool compressDsmFile, IProgress<ProgressInfo> progress);
        Task OpenModel(string dsmFilename, Progress<ProgressInfo> progress);
        Task SaveModel(string dsmFilename, Progress<ProgressInfo> progress);
        bool IsModified { get; }
        bool CanUndo();
        string GetUndoActionDescription();
        void Undo();
        bool CanRedo();
        string GetRedoActionDescription();
        void Redo();
        string GetOverviewReport();
        IEnumerable<IDsmElement> RootElements { get; }
        IEnumerable<IDsmElement> GetElementProvidedElements(IDsmElement element);
        IEnumerable<IDsmElement> GetElementProviders(IDsmElement element);
        IEnumerable<IDsmResolvedRelation> FindResolvedRelations(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider);
        IEnumerable<IDsmElement> GetElementConsumers(IDsmElement element);
        IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider);

        bool IsFirstChild(IDsmElement element);
        bool IsLastChild(IDsmElement element);
        IDsmElement NextSibling(IDsmElement element);
        IDsmElement PreviousSibling(IDsmElement element);
        bool HasChildren(IDsmElement element);
        int GetRecursiveChildCount(IDsmElement element);
        void Sort(IDsmElement element, string algorithm);
        IEnumerable<string> GetSupportedSortAlgorithms();
        void MoveUp(IDsmElement element);
        void MoveDown(IDsmElement element);


        int GetDependencyWeight(IDsmElement consumer, IDsmElement provider);
        bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider);

        IEnumerable<IDsmElement> SearchElements(string text);

        void CreateElement(string name, string type, IDsmElement parent);
        void DeleteElement(IDsmElement element);
        void ChangeElementName(IDsmElement element, string name);
        void ChangeElementType(IDsmElement element, string type);
        void ChangeElementParent(IDsmElement element, IDsmElement newParent);

        void CreateRelation(IDsmElement consumer, IDsmElement provider, string type, int weight);
        void DeleteRelation(IDsmRelation relation);
        void ChangeRelationType(IDsmRelation relation, string type);
        void ChangeRelationWeight(IDsmRelation relation, int weight);

        void MakeSnapshot(string name);

        IEnumerable<IAction> GetActions();
        void ClearActions();
    }
}

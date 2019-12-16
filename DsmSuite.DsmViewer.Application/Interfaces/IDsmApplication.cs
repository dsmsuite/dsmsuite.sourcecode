using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Interfaces
{
    public interface IDsmApplication
    {
        event EventHandler<bool> Modified;
        event EventHandler ActionPerformed;

        void ImportModel(string dsiFilename, string dsmFilename, bool applyPartitionAlgorithm, bool overwriteDsmFile, bool compressDsmFile);
        Task OpenModel(string dsmFilename, Progress<DsmProgressInfo> progress);
        Task SaveModel(string dsmFilename, Progress<DsmProgressInfo> progress);
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
        bool HasChildren(IDsmElement element);
        void Sort(IDsmElement element, string algorithm);
        void MoveUp(IDsmElement element);
        void MoveDown(IDsmElement element);
        IEnumerable<string> GetSupportedSortAlgorithms();

        int GetDependencyWeight(IDsmElement consumer, IDsmElement provider);
        bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider);

        IEnumerable<IDsmElement> SearchElements(string text);

        void CreateElement(string name, string type, IDsmElement parent);
        void DeleteElement(IDsmElement element);
        void EditElement(IDsmElement element, string name, string type);
        void MoveElement(IDsmElement element, IDsmElement newParent);

        void CreateRelation(IDsmElement consumer, IDsmElement provider, string type, int weight);
        void DeleteRelation(IDsmRelation relation);
        void EditRelation(IDsmRelation relation, string type, int weight);

        void MakeSnapshot(string name);

        IEnumerable<IAction> GetActions();
        void ClearActions();
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Application.Actions.Management;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using DsmSuite.DsmViewer.Application.Actions.Snapshot;
using DsmSuite.DsmViewer.Application.Import;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Queries;
using DsmSuite.DsmViewer.Application.Sorting;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Reporting;
using DsmSuite.Analyzer.Model.Core;
using System.Reflection;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Application.Core
{
    public class DsmApplication : IDsmApplication
    {
        private readonly IDsmModel _dsmModel;
        private readonly ActionManager _actionManager;
        private readonly ActionStore _actionStore;
        private readonly DsmQueries _queries;

        public event EventHandler<bool> Modified;
        public event EventHandler ActionPerformed;

        public DsmApplication(IDsmModel dsmModel)
        {
            _dsmModel = dsmModel;

            _actionManager = new ActionManager();
            _actionManager.ActionPerformed += OnActionPerformed;

            _actionStore = new ActionStore(_dsmModel, _actionManager);

            _queries = new DsmQueries(dsmModel);

            ShowCycles = true;
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            ActionPerformed?.Invoke(this, e);
            IsModified = true;
            Modified?.Invoke(this, IsModified);
        }

        public bool CanUndo()
        {
            return _actionManager.CanUndo();
        }

        public string GetUndoActionDescription()
        {
            return _actionManager.GetCurrentUndoAction()?.Description;
        }

        public void Undo()
        {
            _actionManager.Undo();
        }

        public bool CanRedo()
        {
            return _actionManager.CanRedo();
        }

        public string GetRedoActionDescription()
        {
            return _actionManager.GetCurrentRedoAction()?.Description;
        }

        public void Redo()
        {
            _actionManager.Redo();
        }

        public bool ShowCycles { get; set; }

        public void ImportModel(string dsiFilename, string dsmFilename, bool autoPartition, bool overwriteDsmFile, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            string processStep = "Builder";
            Assembly assembly = Assembly.GetEntryAssembly();
            DsiModel dsiModel = new DsiModel(processStep, assembly);
            dsiModel.Load(dsiFilename, progress);

            IImportPolicy importPolicy;
            if (!File.Exists(dsmFilename) || overwriteDsmFile)
            {
                importPolicy = new CreateNewModelPolicy(_dsmModel, autoPartition);
            }
            else
            {
                importPolicy = new UpdateExistingModelPolicy(_dsmModel, dsmFilename, _actionManager, progress);
            }

            DsmBuilder builder = new DsmBuilder(dsiModel, importPolicy);
            builder.Build();
            _actionStore.SaveToModel();
            _dsmModel.SaveModel(dsmFilename, compressDsmFile, progress);
        }

        public async Task OpenModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            await Task.Run(() => _dsmModel.LoadModel(dsmFilename, progress));
            _actionStore.LoadFromModel();
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public async Task SaveModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            _actionStore.SaveToModel();
            await Task.Run(() => _dsmModel.SaveModel(dsmFilename, _dsmModel.IsCompressed, progress));
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public IEnumerable<IDsmElement> RootElements => _dsmModel.GetRootElements();

        public bool IsModified { get; private set; }

        public string GetOverviewReport()
        {
            OverviewReport report = new OverviewReport(_dsmModel);
            return report.WriteReport();
        }

        public IEnumerable<IDsmElement> GetElementConsumers(IDsmElement element)
        {
            return _queries.GetElementConsumers(element);
        }

        public IEnumerable<IDsmElement> GetElementProvidedElements(IDsmElement element)
        {
            return _queries.GetElementProvidedElements(element);
        }

        public IEnumerable<IDsmElement> GetElementProviders(IDsmElement element)
        {
            return _queries.GetElementProviders(element);
        }

        public IEnumerable<IDsmResolvedRelation> FindResolvedRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.FindRelations(consumer, provider);
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.FindRelations(consumer, provider);
        }

        public IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationProviders(consumer, provider);
        }

        public IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationConsumers(consumer, provider);
        }

        public IDsmElement NextSibling(IDsmElement element)
        {
            return _dsmModel.NextSibling(element);
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            return _dsmModel.PreviousSibling(element);
        }

        public bool IsFirstChild(IDsmElement element)
        {
            return _dsmModel.PreviousSibling(element) == null;
       }

        public bool IsLastChild(IDsmElement element)
        {
            return _dsmModel.NextSibling(element) == null;
        }

        public bool HasChildren(IDsmElement element)
        {
            return element?.Children.Count > 0;
        }

        public int GetRecursiveChildCount(IDsmElement element)
        {
            return _dsmModel.GetRecursiveChildCount(element);
        }

        public void Sort(IDsmElement element, string algorithm)
        {
            ElementSortAction action = new ElementSortAction(_dsmModel, element, algorithm);
            _actionManager.Execute(action);
        }

        public IEnumerable<string> GetSupportedSortAlgorithms()
        {
            return SortAlgorithmFactory.GetSupportedAlgorithms();
        }

        public void MoveUp(IDsmElement element)
        {
            ElementMoveUpAction action = new ElementMoveUpAction(_dsmModel, element);
            _actionManager.Execute(action);
        }

        public void MoveDown(IDsmElement element)
        {
            ElementMoveDownAction action = new ElementMoveDownAction(_dsmModel, element);
            _actionManager.Execute(action);
        }
        
        public int GetDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.GetDependencyWeight(consumer.Id, provider.Id);
        }

        public bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.IsCyclicDependency(consumer.Id, provider.Id);
        }

        public IEnumerable<IDsmElement> SearchElements(string text)
        {
            return _dsmModel.SearchElements(text);
        }

        public void CreateElement(string name, string type, IDsmElement parent)
        {
            ElementCreateAction action = new ElementCreateAction(_dsmModel, name, type, parent);
            _actionManager.Execute(action);
        }

        public void DeleteElement(IDsmElement element)
        {
            ElementDeleteAction action = new ElementDeleteAction(_dsmModel, element);
            _actionManager.Execute(action);
        }

        public void ChangeElementName(IDsmElement element, string name)
        {
            ElementChangeNameAction action = new ElementChangeNameAction(_dsmModel, element, name);
            _actionManager.Execute(action);
        }

        public void ChangeElementType(IDsmElement element, string type)
        {
            ElementChangeTypeAction action = new ElementChangeTypeAction(_dsmModel, element, type);
            _actionManager.Execute(action);
        }

        public void ChangeElementParent(IDsmElement element, IDsmElement newParent)
        {
            ElementChangeParentAction action = new ElementChangeParentAction(_dsmModel, element, newParent);
            _actionManager.Execute(action);
        }
        
        public void CreateRelation(IDsmElement consumer, IDsmElement provider, string type, int weight)
        {
            RelationCreateAction action = new RelationCreateAction(_dsmModel, consumer.Id, provider.Id, type, weight);
            _actionManager.Execute(action);
        }

        public void DeleteRelation(IDsmRelation relation)
        {
            RelationDeleteAction action = new RelationDeleteAction(_dsmModel, relation);
            _actionManager.Execute(action);
        }

        public void ChangeRelationType(IDsmRelation relation, string type)
        {
            RelationChangeTypeAction action = new RelationChangeTypeAction(_dsmModel, relation, type);
            _actionManager.Execute(action);
        }

        public void ChangeRelationWeight(IDsmRelation relation, int weight)
        {
            RelationChangeWeightAction action = new RelationChangeWeightAction(_dsmModel, relation, weight);
            _actionManager.Execute(action);
        }

        public void MakeSnapshot(string description)
        {
            MakeSnapshotAction action = new MakeSnapshotAction(_dsmModel, description);
            _actionManager.Execute(action);
        }

        public IEnumerable<IAction> GetActions()
        {
            return _actionManager.GetActionsInReverseChronologicalOrder();
        }

        public void ClearActions()
        {
            _actionManager.Clear();
            IsModified = true;
            Modified?.Invoke(this, IsModified);
        }
    }
}

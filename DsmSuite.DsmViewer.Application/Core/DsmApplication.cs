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
using DsmSuite.Analyzer.Model.Core;
using System.Reflection;
using DsmSuite.Common.Util;
using DsmSuite.DsmViewer.Application.Import.Common;
using DsmSuite.DsmViewer.Application.Import.Dsi;
using DsmSuite.DsmViewer.Application.Metrics;
using DsmSuite.DsmViewer.Application.Import.GraphViz;
using DsmSuite.DsmViewer.Application.Import.GraphML;

namespace DsmSuite.DsmViewer.Application.Core
{
    public class DsmApplication : IDsmApplication
    {
        private readonly IDsmModel _dsmModel;
        private readonly ActionManager _actionManager;
        private readonly ActionStore _actionStore;
        private readonly DsmQueries _queries;
        private readonly DsmMetrics _metrics;

        public event EventHandler<bool> Modified;
        public event EventHandler ActionPerformed;

        public DsmApplication(IDsmModel dsmModel)
        {
            _dsmModel = dsmModel;

            _actionManager = new ActionManager();
            _actionManager.ActionPerformed += OnActionPerformed;

            _actionStore = new ActionStore(_dsmModel, _actionManager);

            _queries = new DsmQueries(dsmModel);

            _metrics = new DsmMetrics();
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
        public bool CaseSensitiveSearch { get; set; }

        public async Task AsyncImportDsiModel(string dsiFilename, string dsmFilename, bool autoPartition, bool recordChanges, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            await Task.Run(() => ImportDsiModel(dsiFilename, dsmFilename, autoPartition, recordChanges, compressDsmFile, progress));
            _actionStore.LoadFromModel();
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public async Task AsyncImportGraphVizModel(string dsiFilename, string dsmFilename, bool autoPartition, bool recordChanges, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            await Task.Run(() => ImportGraphVizModel(dsiFilename, dsmFilename, autoPartition, recordChanges, compressDsmFile, progress));
            _actionStore.LoadFromModel();
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public async Task AsyncImportGraphMlModel(string dsiFilename, string dsmFilename, bool autoPartition, bool recordChanges, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            await Task.Run(() => ImportGraphMlModel(dsiFilename, dsmFilename, autoPartition, recordChanges, compressDsmFile, progress));
            _actionStore.LoadFromModel();
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public void ImportDsiModel(string dsiFilename, string dsmFilename, bool autoPartition, bool recordChanges, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            string processStep = "Builder";
            Assembly assembly = Assembly.GetEntryAssembly();
            DsiModel dsiModel = new DsiModel(processStep, assembly);
            dsiModel.Load(dsiFilename, progress);

            IImportPolicy importPolicy;
            if (!File.Exists(dsmFilename) || !recordChanges)
            {
                importPolicy = new CreateNewModelPolicy(_dsmModel);
            }
            else
            {
                importPolicy = new UpdateExistingModelPolicy(_dsmModel, dsmFilename, _actionManager, progress);
            }

            DsiImporter importer = new DsiImporter(dsiModel, _dsmModel, importPolicy, autoPartition);
            importer.Import(progress);
            _actionStore.SaveToModel();
            _dsmModel.SaveModel(dsmFilename, compressDsmFile, progress);
        }

        public void ImportGraphVizModel(string dotFilename, string dsmFilename, bool autoPartition, bool overwriteDsmFile, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            IImportPolicy importPolicy =  new CreateNewModelPolicy(_dsmModel);

            GraphVizImporter importer = new GraphVizImporter(dotFilename, _dsmModel, importPolicy, autoPartition);
            importer.Import(progress);
            _actionStore.SaveToModel();
            _dsmModel.SaveModel(dsmFilename, compressDsmFile, progress);
        }

        public void ImportGraphMlModel(string graphMlFilename, string dsmFilename, bool applyPartitionAlgorithm, bool overwriteDsmFile, bool compressDsmFile, IProgress<ProgressInfo> progress)
        {
            IImportPolicy importPolicy = new CreateNewModelPolicy(_dsmModel);

            GraphMlImporter importer = new GraphMlImporter(graphMlFilename, _dsmModel, importPolicy, applyPartitionAlgorithm);
            importer.Import(progress);
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

        public IDsmElement RootElement => _dsmModel.GetRootElement();

        public bool IsModified { get; private set; }

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

        public IEnumerable<IDsmRelation> FindResolvedRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.FindRelations(consumer, provider);
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.FindRelations(consumer, provider);
        }

        public IEnumerable<IDsmRelation> FindIngoingRelations(IDsmElement element)
        {
            return _queries.FindIngoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindOutgoingRelations(IDsmElement element)
        {
            return _queries.FindOutgoingRelations(element);
        }

        public IEnumerable<IDsmRelation> FindInternalRelations(IDsmElement element)
        {
            return _queries.FindInternalRelations(element);
        }

        public IEnumerable<IDsmRelation> FindExternalRelations(IDsmElement element)
        {
            return _queries.FindExternalRelations(element);
        }

        public IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationProviders(consumer, provider);
        }

        public IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationConsumers(consumer, provider);
        }

        public int GetHierarchicalCycleCount(IDsmElement element)
        {
            return _dsmModel.GetHierarchicalCycleCount(element);
        }

        public int GetSystemCycleCount(IDsmElement element)
        {
            return _dsmModel.GetSystemCycleCount(element);
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
            return _dsmModel.GetDependencyWeight(consumer, provider);
        }

        public int GetDirectDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.GetDirectDependencyWeight(consumer, provider);
        }

        public CycleType IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.IsCyclicDependency(consumer, provider);
        }

        public int SearchElements(string searchText)
        {
            return _dsmModel.SearchElements(searchText, CaseSensitiveSearch);
        }

        public IDsmElement GetElementByFullname(string text)
        {
            return _dsmModel.GetElementByFullname(text);
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

        public void ChangeElementAnnotation(IDsmElement element, string annotation)
        {
            ElementChangeAnnotationAction action = new ElementChangeAnnotationAction(_dsmModel, element, annotation);
            _actionManager.Execute(action);
        }

        public void ChangeElementType(IDsmElement element, string type)
        {
            ElementChangeTypeAction action = new ElementChangeTypeAction(_dsmModel, element, type);
            _actionManager.Execute(action);
        }

        public void ChangeElementParent(IDsmElement element, IDsmElement newParent, int index)
        {
            if (_dsmModel.IsChangeElementParentAllowed(element, newParent))
            {
                ElementChangeParentAction action = new ElementChangeParentAction(_dsmModel, element, newParent, index);
                _actionManager.Execute(action);
            }
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
            _dsmModel.ClearActions();
            IsModified = true;
            Modified?.Invoke(this, IsModified);
        }

        public int GetElementSize(IDsmElement element)
        {
            return _metrics.GetElementSize(element);
        }

        public int GetElementCount()
        {
            return _dsmModel.GetElementCount();
        }

        public void AddElementAnnotation(IDsmElement element, string text)
        {
            _dsmModel.ChangeElementAnnotation(element, text);
        }

        public void AddRelationAnnotation(IDsmElement consumer, IDsmElement provider, string text)
        {
            _dsmModel.ChangeRelationAnnotation(consumer, provider, text);
        }

        public IDsmElementAnnotation FindElementAnnotation(IDsmElement element)
        {
            return _dsmModel.FindElementAnnotation(element);
        }

        public IDsmRelationAnnotation FindRelationAnnotation(IDsmElement consumer, IDsmElement provider)
        {
            return _dsmModel.FindRelationAnnotation(consumer, provider);
        }
    }
}

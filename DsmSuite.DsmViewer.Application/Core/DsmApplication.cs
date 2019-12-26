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
using DsmSuite.DsmViewer.Model.Core;
using System.Reflection;
using DsmSuite.Common.Util;

namespace DsmSuite.DsmViewer.Application.Core
{
    public class DsmApplication : IDsmApplication
    {
        private readonly IDsmModel _model;
        private readonly ActionManager _actionManager;
        private readonly ActionStore _actionStore;
        private readonly DsmQueries _queries;

        public event EventHandler<bool> Modified;
        public event EventHandler ActionPerformed;

        public DsmApplication(IDsmModel model)
        {
            _model = model;

            _actionManager = new ActionManager();
            _actionManager.ActionPerformed += OnActionPerformed;

            _actionStore = new ActionStore(_model, _actionManager);

            _queries = new DsmQueries(model);

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

        public void ImportModel(string dsiFilename, string dsmFilename, bool autoPartition, bool overwriteDsmFile, bool compressDsmFile)
        {
            string processStep = "Builder";
            Assembly assembly = Assembly.GetEntryAssembly();
            DsiModel dsiModel = new DsiModel(processStep, assembly);
            dsiModel.Load(dsiFilename, null);
            DsmModel dsmModel = new DsmModel(processStep, assembly);

            IImportPolicy importPolicy;
            if (!File.Exists(dsmFilename) || overwriteDsmFile)
            {
                importPolicy = new CreateNewModelPolicy(dsmModel, autoPartition);
            }
            else
            {
                importPolicy = new UpdateExistingModelPolicy(dsmModel, dsmFilename, _actionManager);
            }

            DsmBuilder builder = new DsmBuilder(dsiModel, importPolicy);
            builder.Build();
            dsmModel.SaveModel(dsmFilename, compressDsmFile, null);
        }

        public async Task OpenModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            await Task.Run(() => _model.LoadModel(dsmFilename, progress));
            _actionStore.Load();
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public async Task SaveModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            _actionStore.Save();
            await Task.Run(() => _model.SaveModel(dsmFilename, _model.IsCompressed, progress));
            IsModified = false;
            Modified?.Invoke(this, IsModified);
        }

        public IEnumerable<IDsmElement> RootElements => _model.GetRootElements();

        public bool IsModified { get; private set; }

        public string GetOverviewReport()
        {
            OverviewReport report = new OverviewReport(_model);
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
            return _model.FindRelations(consumer, provider);
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
            return _model.NextSibling(element);
        }

        public IDsmElement PreviousSibling(IDsmElement element)
        {
            return _model.PreviousSibling(element);
        }

        public bool IsFirstChild(IDsmElement element)
        {
            return _model.PreviousSibling(element) == null;
       }

        public bool IsLastChild(IDsmElement element)
        {
            return _model.NextSibling(element) == null;
        }

        public bool HasChildren(IDsmElement element)
        {
            return element?.Children.Count > 0;
        }

        public void Sort(IDsmElement element, string algorithm)
        {
            ElementSortAction action = new ElementSortAction(_model, element, algorithm);
            _actionManager.Execute(action);
        }

        public IEnumerable<string> GetSupportedSortAlgorithms()
        {
            return SortAlgorithmFactory.GetSupportedAlgorithms();
        }

        public void MoveUp(IDsmElement element)
        {
            ElementMoveUpAction action = new ElementMoveUpAction(_model, element);
            _actionManager.Execute(action);
        }

        public void MoveDown(IDsmElement element)
        {
            ElementMoveDownAction action = new ElementMoveDownAction(_model, element);
            _actionManager.Execute(action);
        }
        
        public int GetDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _model.GetDependencyWeight(consumer.Id, provider.Id);
        }

        public bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _model.IsCyclicDependency(consumer.Id, provider.Id);
        }

        public IEnumerable<IDsmElement> SearchElements(string text)
        {
            return _model.SearchElements(text);
        }

        public void CreateElement(string name, string type, IDsmElement parent)
        {
            ElementCreateAction action = new ElementCreateAction(_model, name, type, parent);
            _actionManager.Execute(action);
        }

        public void DeleteElement(IDsmElement element)
        {
            ElementDeleteAction action = new ElementDeleteAction(_model, element);
            _actionManager.Execute(action);
        }

        public void ChangeElementName(IDsmElement element, string name)
        {
            ElementChangeNameAction action = new ElementChangeNameAction(_model, element, name);
            _actionManager.Execute(action);
        }

        public void ChangeElementType(IDsmElement element, string type)
        {
            ElementChangeTypeAction action = new ElementChangeTypeAction(_model, element, type);
            _actionManager.Execute(action);
        }

        public void ChangeElementParent(IDsmElement element, IDsmElement newParent)
        {
            ElementChangeParentAction action = new ElementChangeParentAction(_model, element, newParent);
            _actionManager.Execute(action);
        }
        
        public void CreateRelation(IDsmElement consumer, IDsmElement provider, string type, int weight)
        {
            RelationCreateAction action = new RelationCreateAction(_model, consumer.Id, provider.Id, type, weight);
            _actionManager.Execute(action);
        }

        public void DeleteRelation(IDsmRelation relation)
        {
            RelationDeleteAction action = new RelationDeleteAction(_model, relation);
            _actionManager.Execute(action);
        }

        public void ChangeRelationType(IDsmRelation relation, string type)
        {
            RelationChangeTypeAction action = new RelationChangeTypeAction(_model, relation, type);
            _actionManager.Execute(action);
        }

        public void ChangeRelationWeight(IDsmRelation relation, int weight)
        {
            RelationChangeWeightAction action = new RelationChangeWeightAction(_model, relation, weight);
            _actionManager.Execute(action);
        }

        public void MakeSnapshot(string description)
        {
            MakeSnapshotAction action = new MakeSnapshotAction(_model, description);
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

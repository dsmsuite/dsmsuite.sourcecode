using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Actions.Relation;
using DsmSuite.DsmViewer.Application.Import;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Application.Queries;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Reporting;

namespace DsmSuite.DsmViewer.Application.Core
{
    public class DsmApplication : IDsmApplication
    {
        private readonly IDsmModel _model;
        private readonly ActionManager _actionManager;
        private readonly DsmQueries _queries;

        public event EventHandler<bool> Modified;
        public event EventHandler ActionPerformed;

        public DsmApplication(IDsmModel model)
        {
            _model = model;
            _model.Modified += OnModelModified;

            _actionManager = new ActionManager();
            _actionManager.ActionPerformed += OnActionPerformed;

            _queries = new DsmQueries(model);
        }

        private void OnActionPerformed(object sender, EventArgs e)
        {
            ActionPerformed?.Invoke(sender, e);
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

        private void OnModelModified(object sender, bool e)
        {
            Modified?.Invoke(sender,e);
        }

        public void ImportModel(string dsiFilename, string dsmFilename, bool applyPartitionAlgorithm, bool overwriteDsmFile, bool compressDsmFile)
        {
            if (!File.Exists(dsmFilename) || overwriteDsmFile)
            {
                DsmBuilder builder = new DsmBuilder(_model);
                builder.Build(dsiFilename, dsmFilename, applyPartitionAlgorithm, compressDsmFile);
            }
            else
            {
                DsmUpdater updater = new DsmUpdater(_model);
                updater.Update(dsiFilename, dsmFilename, compressDsmFile);
            }
        }

        public async Task OpenModel(string dsmFilename, Progress<DsmProgressInfo> progress)
        {
            await Task.Run(() => _model.LoadModel(dsmFilename, progress));
        }

        public async Task SaveModel(string dsmFilename, Progress<DsmProgressInfo> progress)
        {
            await Task.Run(() => _model.SaveModel(dsmFilename, _model.IsCompressed, progress));
        }

        public IEnumerable<IDsmElement> RootElements => _model.RootElements;

        public bool IsModified => _model.IsModified;

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

        public IEnumerable<IDsmResolvedRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.FindRelations(consumer, provider);
        }

        public IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationProviders(consumer, provider);
        }

        public IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider)
        {
            return _queries.GetRelationConsumers(consumer, provider);
        }

        public bool IsFirstChild(IDsmElement element)
        {
            return element?.PreviousSibling == null;
       }

        public bool IsLastChild(IDsmElement element)
        {
            return element?.NextSibling == null;
        }

        public bool HasChildren(IDsmElement element)
        {
            return element?.Children.Count > 0;
        }

        public void Sort(IDsmElement element, string algorithm)
        {
            ElementPartitionAction action = new ElementPartitionAction(_model, element, algorithm);
            _actionManager.Execute(action);
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

        public IEnumerable<string> GetSupportedSortAlgorithms()
        {
            return new List<string> {"Partition"};
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
            ElementCreateAction action = new ElementCreateAction(_model, name, type, parent.Id);
            _actionManager.Execute(action);
        }

        public void DeleteElement(IDsmElement element)
        {
            ElementDeleteAction action = new ElementDeleteAction(_model, element);
            _actionManager.Execute(action);
        }

        public void RenameElement(IDsmElement element, string newName)
        {
            ElementRenameAction action = new ElementRenameAction(_model, element, newName);
            _actionManager.Execute(action);
        }

        public void MoveElement(IDsmElement element, IDsmElement newParent)
        {
            ElementMoveAction action = new ElementMoveAction(_model, element, newParent);
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Application.Actions.Base;
using DsmSuite.DsmViewer.Application.Actions.Element;
using DsmSuite.DsmViewer.Application.Import;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;
using DsmSuite.DsmViewer.Reporting;

namespace DsmSuite.DsmViewer.Application.Core
{
    public class DsmApplication : IDsmApplication
    {
        private readonly IDsmModel _model;
        private readonly ActionManager _actionManager;

        public event EventHandler<bool> Modified;

        public DsmApplication(IDsmModel model)
        {
            _model = model;
            _model.Modified += OnModelModified;

            _actionManager = new ActionManager();
        }

        public void Undo()
        {
            _actionManager.Undo();
        }

        public void Redo()
        {
            _actionManager.Redo();
        }

        private void OnModelModified(object sender, bool e)
        {
            Modified?.Invoke(sender,e);
        }

        public void ImportModel(string dsiFilename, string dsmFilename, bool overwriteDsmFile, bool compressDsmFile)
        {
            DsmBuilder builder = new DsmBuilder(_model);
            builder.BuildModel(dsiFilename, dsmFilename, overwriteDsmFile, compressDsmFile);
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

        public IEnumerable<IDsmElement> GetElementProvidedElements(IDsmElement element)
        {
            var relations = _model.ResolveRelations(_model.FindProviderRelations(element))
                .OrderBy(x => x.Provider.Fullname)
                .GroupBy(x => x.Provider.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.Provider)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmElement> GetElementProviders(IDsmElement element)
        {
            var relations = _model.ResolveRelations(_model.FindConsumerRelations(element))
                .OrderBy(x => x.Provider.Fullname)
                .GroupBy(x => x.Provider.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.Provider)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmResolvedRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            var relations = _model.ResolveRelations(_model.FindRelations(consumer, provider))
                .OrderBy(x => x.Provider.Fullname)
                .ThenBy(x => x.Consumer.Fullname)
                .ToList();
            return relations;
        }

        public IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            var relations = _model.ResolveRelations(_model.FindRelations(consumer, provider))
                .OrderBy(x => x.Provider.Fullname)
                .GroupBy(x => x.Provider.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.Provider)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider)
        {
            var relations = _model.ResolveRelations(_model.FindRelations(consumer, provider))
                .OrderBy(x => x.Consumer.Fullname)
                .GroupBy(x => x.Consumer.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.Consumer)
                .ToList();
            return elements;
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

        public IEnumerable<IDsmElement> GetElementConsumers(IDsmElement element)
        {
            var relations = _model.ResolveRelations(_model.FindProviderRelations(element))
                .OrderBy(x => x.Consumer.Fullname)
                .GroupBy(x => x.Consumer.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.Consumer)
                .ToList();
            return elements;
        }

        public int GetDependencyWeight(IDsmElement consumer, IDsmElement provider)
        {
            return _model.GetDependencyWeight(consumer.Id, provider.Id);
        }

        public bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _model.IsCyclicDependency(consumer.Id, provider.Id);
        }

        public IEnumerable<IDsmElement> SearchExecute(string text)
        {
            return _model.GetElementsWithFullnameContainingText(text);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DsmSuite.DsmViewer.Model;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application
{
    public class DsmApplication : IDsmApplication
    {
        private readonly IDsmModel _model;

        public DsmApplication(IDsmModel model)
        {
            _model = model;
        }

        public async Task ImportModel(string dsiFilename, string dsmFilename, bool overwrite, Progress<ProgressInfo> progress)
        {
            throw new NotImplementedException();
        }

        public async Task OpenModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            await Task.Run(() => _model.LoadModel(dsmFilename, progress));
        }

        public async Task SaveModel(string dsmFilename, Progress<ProgressInfo> progress)
        {
            await Task.Run(() => _model.SaveModel(dsmFilename, _model.IsCompressed, progress));
        }

        public IList<IDsmElement> RootElements => _model.RootElements;

        public IDsmModel Model
        {
            get { return _model; }
        }

        public IEnumerable<IDsmElement> GetElementProvidedElements(IDsmElement element)
        {
            var relations = _model.FindProviderRelations(element)
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
            var relations = _model.FindConsumerRelations(element)
                .OrderBy(x => x.Provider.Fullname)
                .GroupBy(x => x.Provider.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.Provider)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            var relations = _model.FindRelations(consumer, provider)
                .OrderBy(x => x.Provider.Fullname)
                .ThenBy(x => x.Consumer.Fullname)
                .ToList();
            return relations;
        }

        public IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            var relations = _model.FindRelations(consumer, provider)
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
            var relations = _model.FindRelations(consumer, provider)
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
            _model.Partition(element);
        }

        public IEnumerable<string> GetSupportedSortAlgorithms()
        {
            return new List<string> {"Partition"};
        }

        public IEnumerable<IDsmElement> GetElementConsumers(IDsmElement element)
        {
            var relations = _model.FindProviderRelations(element)
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
            return _model.GetDependencyWeight(consumer, provider);
        }

        public bool IsCyclicDependency(IDsmElement consumer, IDsmElement provider)
        {
            return _model.IsCyclicDependency(consumer, provider);
        }

        public IEnumerable<IDsmElement> SearchExecute(string text)
        {
            return _model.GetElementsWithFullnameContainingText(text);
        }
    }
}

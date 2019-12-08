using System.Collections.Generic;
using System.Linq;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.Model.Interfaces;

namespace DsmSuite.DsmViewer.Application.Queries
{
    public class DsmQueries
    {
        private readonly IDsmModel _model;
        public DsmQueries(IDsmModel model)
        {
            _model = model;
        }

        public IEnumerable<IDsmElement> GetRelationConsumers(IDsmElement consumer, IDsmElement provider)
        {
            var relations = ResolveRelations(_model.FindRelations(consumer, provider))
                .OrderBy(x => x.ConsumerElement.Fullname)
                .GroupBy(x => x.ConsumerElement.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.ConsumerElement)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmElement> GetElementProvidedElements(IDsmElement element)
        {
            var relations = ResolveRelations(_model.FindProviderRelations(element))
                .OrderBy(x => x.ProviderElement.Fullname)
                .GroupBy(x => x.ProviderElement.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.ProviderElement)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmElement> GetElementProviders(IDsmElement element)
        {
            var relations = ResolveRelations(_model.FindConsumerRelations(element))
                .OrderBy(x => x.ProviderElement.Fullname)
                .GroupBy(x => x.ProviderElement.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.ProviderElement)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmElement> GetElementConsumers(IDsmElement element)
        {
            var relations = ResolveRelations(_model.FindProviderRelations(element))
                .OrderBy(x => x.ConsumerElement.Fullname)
                .GroupBy(x => x.ConsumerElement.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.ConsumerElement)
                .ToList();
            return elements;
        }

        public IEnumerable<IDsmResolvedRelation> FindRelations(IDsmElement consumer, IDsmElement provider)
        {
            var relations = ResolveRelations(_model.FindRelations(consumer, provider))
                .OrderBy(x => x.ProviderElement.Fullname)
                .ThenBy(x => x.ConsumerElement.Fullname)
                .ToList();
            return relations;
        }

        public IEnumerable<IDsmElement> GetRelationProviders(IDsmElement consumer, IDsmElement provider)
        {
            var relations = ResolveRelations(_model.FindRelations(consumer, provider))
                .OrderBy(x => x.ProviderElement.Fullname)
                .GroupBy(x => x.ProviderElement.Fullname)
                .Select(x => x.FirstOrDefault())
                .ToList();

            var elements = relations.Select(x => x.ProviderElement)
                .ToList();
            return elements;
        }

        private IEnumerable<IDsmResolvedRelation> ResolveRelations(IEnumerable<IDsmRelation> relations)
        {
            List<IDsmResolvedRelation> resolvedRelations = new List<IDsmResolvedRelation>();
            foreach (IDsmRelation relation in relations)
            {
                resolvedRelations.Add(new DsmResolvedRelation(_model, relation));
            }
            return resolvedRelations;
        }
    }
}

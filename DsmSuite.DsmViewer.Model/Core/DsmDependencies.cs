using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmDependencies
    {
        private readonly Dictionary<int /*providerId*/, int /*weight*/> _directWeights = new Dictionary<int, int>();
        private readonly Dictionary<int /*providerId*/, int /*weight*/> _derivedWeights = new Dictionary<int, int>();
        private readonly Dictionary<int /*providerId*/, Dictionary<string /*type*/, DsmRelation>> _outgoingRelations = new Dictionary<int, Dictionary<string, DsmRelation>>();
        private readonly Dictionary<int /*consumerId*/, Dictionary<string /*type*/, DsmRelation>> _ingoingRelations = new Dictionary<int, Dictionary<string, DsmRelation>>();
        private readonly IDsmElement _element;

        public DsmDependencies(IDsmElement element)
        {
            _element = element;
        }

        public void AddIngoingRelation(DsmRelation relation)
        {
            if (!_ingoingRelations.ContainsKey(relation.Consumer.Id))
            {
                _ingoingRelations[relation.Consumer.Id] = new Dictionary<string, DsmRelation>();
            }

            _ingoingRelations[relation.Consumer.Id][relation.Type] = relation;
        }

        public void RemoveIngoingRelation(DsmRelation relation)
        {
            if (_ingoingRelations.ContainsKey(relation.Consumer.Id) &&
                _ingoingRelations[relation.Consumer.Id].ContainsKey(relation.Type))
            {
                _ingoingRelations[relation.Consumer.Id].Remove(relation.Type);
            }
        }

        public IEnumerable<DsmRelation> GetIngoingRelations()
        {
            List<DsmRelation> relations = new List<DsmRelation>();
            foreach (Dictionary<string, DsmRelation> r in _ingoingRelations.Values)
            {
                relations.AddRange(r.Values);
            }
            return relations;
        }

        public IEnumerable<DsmRelation> GetIngoingRelations(IDsmElement consumer)
        {
            if (_ingoingRelations.ContainsKey(consumer.Id))
            {
                return _ingoingRelations[consumer.Id].Values;
            }
            else
            {
                return new List<DsmRelation>();
            }
        }

        public DsmRelation GetIngoingRelation(IDsmElement consumer, string type)
        {
            DsmRelation relation = null;
            if (_ingoingRelations.ContainsKey(consumer.Id))
            {
                _ingoingRelations[consumer.Id].TryGetValue(type, out relation);
            }
            return relation;
        }

        public void AddOutgoingRelation(DsmRelation relation)
        {
            if (!_outgoingRelations.ContainsKey(relation.Provider.Id))
            {
                _outgoingRelations[relation.Provider.Id] = new Dictionary<string, DsmRelation>();
            }

            _outgoingRelations[relation.Provider.Id][relation.Type] = relation;

            AddDirectWeight(relation.Provider, relation.Weight);
        }

        public void RemoveOutgoingRelation(DsmRelation relation)
        {
            if (_outgoingRelations.ContainsKey(relation.Provider.Id) &&
                _outgoingRelations[relation.Provider.Id].ContainsKey(relation.Type))
            {
                _outgoingRelations[relation.Provider.Id].Remove(relation.Type);
            }

            RemoveDirectWeight(relation.Provider, relation.Weight);
        }

        public IEnumerable<DsmRelation> GetOutgoingRelations()
        {
            List<DsmRelation> relations = new List<DsmRelation>();
            foreach(Dictionary<string, DsmRelation> r in _outgoingRelations.Values)
            {
                relations.AddRange(r.Values);
            }
            return relations;
        }

        public IEnumerable<DsmRelation> GetOutgoingRelations(IDsmElement provider)
        {
            if (_outgoingRelations.ContainsKey(provider.Id))
            {
                return _outgoingRelations[provider.Id].Values;
            }
            else
            {
                return new List<DsmRelation>();
            }
        }

        public DsmRelation GetOutgoingRelation(IDsmElement provider, string type)
        {
            DsmRelation relation = null;
            if (_outgoingRelations.ContainsKey(provider.Id))
            {
                _outgoingRelations[provider.Id].TryGetValue(type, out relation);
            }
            return relation;
        }

        public void AddDerivedWeight(IDsmElement provider, int weight)
        {
            int currentWeight = 0;
            if (_derivedWeights.TryGetValue(provider.Id, out currentWeight))
            {
                _derivedWeights[provider.Id] = currentWeight + weight;
            }
            else
            {
                _derivedWeights[provider.Id] = weight;
            }
        }

        public void RemoveDerivedWeight(IDsmElement provider, int weight)
        {
            int currentWeight = 0;
            if (_derivedWeights.TryGetValue(provider.Id, out currentWeight))
            {
                if (currentWeight >= weight)
                {
                    _derivedWeights[provider.Id] = currentWeight - weight;
                }
            }
        }

        public int GetDerivedDependencyWeight(IDsmElement provider)
        {
            int weight = 0;
            if (_element.Id != provider.Id) 
            {
                _derivedWeights.TryGetValue(provider.Id, out weight);
            }
            return weight;
        }

        public int GetDirectDependencyWeight(IDsmElement provider)
        {
            int weight = 0;
            if (_element.Id != provider.Id)
            {
                _directWeights.TryGetValue(provider.Id, out weight);
            }
            return weight;
        }

        private void AddDirectWeight(IDsmElement provider, int weight)
        {
            if (_directWeights.ContainsKey(provider.Id))
            {
                _directWeights[provider.Id] += weight;
            }
            else
            {
                _directWeights[provider.Id] = weight;
            }
        }

        private void RemoveDirectWeight(IDsmElement provider, int weight)
        {
            if (_directWeights.ContainsKey(provider.Id))
            {
                _directWeights[provider.Id] -= weight;
            }
        }
    }
}

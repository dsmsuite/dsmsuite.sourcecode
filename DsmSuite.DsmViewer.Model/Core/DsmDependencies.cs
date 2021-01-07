using DsmSuite.DsmViewer.Model.Interfaces;
using System.Collections.Generic;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmDependencies
    {
        private readonly Dictionary<int /*providerId*/, int /*weight*/> _directWeights = new Dictionary<int, int>();
        private readonly Dictionary<int /*providerId*/, int /*weight*/> _derivedWeights = new Dictionary<int, int>();
        private readonly Dictionary<int /*providerId*/, List<DsmRelation>> _outgoingRelations = new Dictionary<int, List<DsmRelation>>();
        private readonly Dictionary<int /*consumerId*/, List<DsmRelation>> _ingoingRelations = new Dictionary<int, List<DsmRelation>>();
        private readonly IDsmElement _element;

        public DsmDependencies(IDsmElement element)
        {
            _element = element;
        }

        public void AddIngoingRelation(DsmRelation relation)
        {
            if (!_ingoingRelations.ContainsKey(relation.Consumer.Id))
            {
                _ingoingRelations[relation.Consumer.Id] = new List<DsmRelation>();
            }

            _ingoingRelations[relation.Consumer.Id].Add(relation);
        }

        public void RemoveIngoingRelation(DsmRelation relation)
        {
            if (_ingoingRelations.ContainsKey(relation.Consumer.Id))
            {
                _ingoingRelations[relation.Consumer.Id].Remove(relation);
            }
        }

        public IEnumerable<DsmRelation> GetIngoingRelations()
        {
            List<DsmRelation> relations = new List<DsmRelation>();
            foreach (List<DsmRelation> r in _ingoingRelations.Values)
            {
                relations.AddRange(r);
            }
            return relations;
        }

        public void AddOutgoingRelation(DsmRelation relation)
        {
            if (!_outgoingRelations.ContainsKey(relation.Provider.Id))
            {
                _outgoingRelations[relation.Provider.Id] = new List<DsmRelation>();
            }

            _outgoingRelations[relation.Provider.Id].Add(relation);

            AddDirectWeight(relation.Provider, relation.Weight);
        }

        public void RemoveOutgoingRelation(DsmRelation relation)
        {
            if (_outgoingRelations.ContainsKey(relation.Provider.Id))
            {
                _outgoingRelations[relation.Provider.Id].Remove(relation);
            }

            RemoveDirectWeight(relation.Provider, relation.Weight);
        }

        public IEnumerable<DsmRelation> GetOutgoingRelations()
        {
            List<DsmRelation> relations = new List<DsmRelation>();
            foreach(List<DsmRelation> r in _outgoingRelations.Values)
            {
                relations.AddRange(r);
            }
            return relations;
        }

        public IEnumerable<DsmRelation> GetOutgoingRelations(IDsmElement provider)
        {
            if (_outgoingRelations.ContainsKey(provider.Id))
            {
                return _outgoingRelations[provider.Id];
            }
            else
            {
                return new List<DsmRelation>();
            }
        }

        public void AddDerivedWeight(IDsmElement provider, int weight)
        {
            int currentWeight;
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
            int currentWeight;
            if (_derivedWeights.TryGetValue(provider.Id, out currentWeight))
            {
                if (currentWeight >= weight)
                {
                    _derivedWeights[provider.Id] = currentWeight - weight;
                }
                else
                {
                    
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

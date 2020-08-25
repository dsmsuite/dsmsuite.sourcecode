using DsmSuite.DsmViewer.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsmSuite.DsmViewer.Model.Core
{
    public class DsmDependencies
    {
        private readonly Dictionary<int /*providerId*/, int /*weight*/> _directWeights = new Dictionary<int, int>();
        private readonly Dictionary<int /*providerId*/, int /*weight*/> _derivedWeights = new Dictionary<int, int>();
        private readonly IDsmElement _element;

        public DsmDependencies(IDsmElement element)
        {
            _element = element;
        }

        public void AddDirectWeight(IDsmElement provider, int weight)
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

        public void RemoveDirectWeight(IDsmElement provider, int weight)
        {
            if (_directWeights.ContainsKey(provider.Id))
            {
                _directWeights[provider.Id] -= weight;
            }
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
    }
}

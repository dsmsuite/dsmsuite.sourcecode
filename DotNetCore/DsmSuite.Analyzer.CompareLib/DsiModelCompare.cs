using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.CompareLib
{
    public class DsiModelCompare
    {
        private readonly IDsiModel _oldModel;
        private readonly IDsiModel _newModel;
        private readonly IProgress<ProgressInfo> _progress;

        private readonly HashSet<string> _addedElements = new HashSet<string>();
        private readonly HashSet<string> _removedElements = new HashSet<string>();

        private readonly HashSet<string> _allFoundElements = new HashSet<string>();
        private readonly HashSet<string> _oldModelFoundElements = new HashSet<string>();
        private readonly HashSet<string> _newModelFoundElements = new HashSet<string>();

        private readonly Dictionary<string, HashSet<string>> _addedRelations = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _removedRelations = new Dictionary<string, HashSet<string>>();

        private readonly Dictionary<string, HashSet<string>> _allFoundRelations = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _oldModelFoundRelations = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _newModelFoundRelations = new Dictionary<string, HashSet<string>>();

        public DsiModelCompare(IDsiModel oldModel, IDsiModel newModel, IProgress<ProgressInfo> progress)
        {
            _oldModel = oldModel;
            _newModel = newModel;
            _progress = progress;
        }

        public void Compare()
        {
            FindElements();
            CompareElements();

            FindRelations();
            CompareElementRelations();
        }

        public bool AreIdentical =>
            (AddedElementCount == 0) &&
            (RemovedElementCount == 0) &&
            (AddedRelationCount == 0) &&
            (RemovedRelationCount == 0);

        public ISet<string> AddedElements => _addedElements;
        public ISet<string> RemovedElements => _removedElements;

        public int AddedElementCount => _addedElements.Count;
        public int RemovedElementCount => _removedElements.Count;

        public IReadOnlyDictionary<string, HashSet<string>> AddedRelations => _addedRelations;
        public IReadOnlyDictionary<string, HashSet<string>> RemovedRelations => _removedRelations;

        public int AddedRelationCount
        {
            get
            {
                int addedRelationCount = 0;
                foreach (HashSet<string> consumerRelations in _addedRelations.Values)
                {
                    addedRelationCount += consumerRelations.Count;
                }
                return addedRelationCount;
            }
        }

        public int RemovedRelationCount
        {
            get
            {
                int removedRelationCount = 0;
                foreach (HashSet<string> consumerRelations in _removedRelations.Values)
                {
                    removedRelationCount += consumerRelations.Count;
                }
                return removedRelationCount;
            }
        }

        private void FindElements()
        {
            foreach (IDsiElement element in _oldModel.GetElements())
            {
                _oldModelFoundElements.Add(element.Name);
                _allFoundElements.Add(element.Name);
            }

            foreach (IDsiElement element in _newModel.GetElements())
            {
                _newModelFoundElements.Add(element.Name);
                _allFoundElements.Add(element.Name);
            }
        }

        private void CompareElements()
        {
            int totalItemCount = _allFoundElements.Count;
            int currentItemCount = 0;
            foreach (string elementName in _allFoundElements)
            {
                CompareElement(elementName);

                currentItemCount++;
                UpdateProgress("Comparing elements", currentItemCount, totalItemCount, "elements");
            }
        }

        private void CompareElement(string elementName)
        {
            if (!_oldModelFoundElements.Contains(elementName))
            {
                _addedElements.Add(elementName);
            }

            if (!_newModelFoundElements.Contains(elementName))
            {
                _removedElements.Add(elementName);
            }
        }

        private void FindRelations()
        {
            foreach (string consumerName in _allFoundElements)
            {
                _allFoundRelations[consumerName] = new HashSet<string>();
                _oldModelFoundRelations[consumerName] = new HashSet<string>();
                _newModelFoundRelations[consumerName] = new HashSet<string>();

                foreach (IDsiRelation relation in _oldModel.GetRelations())
                {
                    IDsiElement consumer = _oldModel.FindElementById(relation.ConsumerId);
                    IDsiElement provider = _oldModel.FindElementById(relation.ProviderId);

                    if ((consumer != null) && (provider != null) && (consumer.Name == consumerName))
                    {
                        _oldModelFoundRelations[consumerName].Add(provider.Name);
                        _allFoundRelations[consumerName].Add(provider.Name);
                    }
                }

                foreach (IDsiRelation relation in _newModel.GetRelations())
                {
                    IDsiElement consumer = _oldModel.FindElementById(relation.ConsumerId);
                    IDsiElement provider = _oldModel.FindElementById(relation.ProviderId);

                    if ((consumer != null) && (provider != null) && (consumer.Name == consumerName))
                    {
                        _newModelFoundRelations[consumerName].Add(provider.Name);
                        _allFoundRelations[consumerName].Add(provider.Name);
                    }
                }
            }
        }

        private void CompareElementRelations()
        {
            int totalItemCount = _allFoundElements.Count;
            int currentItemCount = 0;
            foreach (string elementName in _allFoundElements)
            {
                CompareElementRelations(elementName);

                currentItemCount++;
                UpdateProgress("Comparing element relations", currentItemCount, totalItemCount, "elements");
            }
        }

        private void CompareElementRelations(string consumerName)
        {
            if (_allFoundRelations.ContainsKey(consumerName))
            {
                _addedRelations[consumerName] = new HashSet<string>();
                _removedRelations[consumerName] = new HashSet<string>();

                foreach (string providerName in _allFoundRelations[consumerName])
                {
                    if (!_oldModelFoundRelations.ContainsKey(consumerName) ||
                        !_oldModelFoundRelations[consumerName].Contains(providerName))
                    {
                        _addedRelations[consumerName].Add(providerName);
                    }

                    if (!_newModelFoundRelations.ContainsKey(consumerName) ||
                        !_newModelFoundRelations[consumerName].Contains(providerName))
                    {
                        _removedRelations[consumerName].Add(providerName);
                    }

                }
            }
        }
        
        private void UpdateProgress(string actionText, int currentItemCount, int totalItemCount, string itemType)
        {
            ProgressInfo progressInfo = new ProgressInfo
            {
                ActionText = actionText,
                CurrentItemCount = currentItemCount,
                TotalItemCount = totalItemCount,
                ItemType = itemType,
                Percentage = currentItemCount * 100 / totalItemCount,
                Done = currentItemCount == totalItemCount
            };
            _progress?.Report(progressInfo);
        }
    }
}

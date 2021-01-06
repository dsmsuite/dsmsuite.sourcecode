using System;
using System.Collections.Generic;
using DsmSuite.Analyzer.Model.Interface;
using DsmSuite.Common.Util;

namespace DsmSuite.Analyzer.Compare
{
    class Comparer
    {
        private readonly IDsiModel _oldModel;
        private readonly IDsiModel _newModel;
        private readonly IProgress<ProgressInfo> _progress;

        private readonly HashSet<string> _oldModelFoundElements = new HashSet<string>();
        private readonly HashSet<string> _newModelFoundElements = new HashSet<string>();
        private readonly HashSet<string> _oldModelMissingElements = new HashSet<string>();
        private readonly HashSet<string> _newModelMissingElements = new HashSet<string>();
        private readonly HashSet<string> _allElements = new HashSet<string>();

        private readonly Dictionary<string, HashSet<string>> _oldModelFoundRelations = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _newModelFoundRelations = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _oldModelMissingRelations = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _newModelMissingRelations = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> _allRelations = new Dictionary<string, HashSet<string>>();

        public Comparer(IDsiModel oldModel, IDsiModel newModel, IProgress<ProgressInfo> progress)
        {
            _oldModel = oldModel;
            _newModel = newModel;
            _progress = progress;
        }

        public void Compare()
        {
            FindElements();

            int totalItemCount = _allElements.Count;

            int currentItemCount = 0;
            foreach (string elementName in _allElements)
            {
                CompareElement(elementName);

                currentItemCount++;
                UpdateProgress("Comparing elements", currentItemCount, totalItemCount, "elements");
            }

            currentItemCount = 0;
            foreach (string elementName in _allElements)
            {
                CompareElementRelations(elementName);

                currentItemCount++;
                UpdateProgress("Comparing element relations", currentItemCount, totalItemCount, "elements");
            }

            if (AreIdentical())
            {
                Logger.LogUserMessage("Models are identical");
            }
            else
            {
                Logger.LogUserMessage("Models are different");
                ReportDeltas();
            }
        }

        private void ReportDeltas()
        {
            Logger.LogUserMessage("Added Elements");
            Logger.LogUserMessage("==============");
            foreach (string elementName in _oldModelMissingElements)
            {
                Logger.LogUserMessage($" {elementName}");
            }

            Logger.LogUserMessage("");

            Logger.LogUserMessage("Removed Elements");
            Logger.LogUserMessage("===============");
            foreach (string elementName in _newModelMissingElements)
            {
                Logger.LogUserMessage($" {elementName}");
            }

            Logger.LogUserMessage("");

            Logger.LogUserMessage("Added Relations");
            Logger.LogUserMessage("==============");
            foreach (KeyValuePair<string, HashSet<string>> relations in _oldModelFoundRelations)
            {
                string consumer = relations.Key;
                foreach (string provider in relations.Value)
                {
                    Logger.LogUserMessage($" {consumer} -> {provider}");
                }
            }

            Logger.LogUserMessage("");

            Logger.LogUserMessage("Removed Relations");
            Logger.LogUserMessage("=================");
            foreach (KeyValuePair<string, HashSet<string>> relations in _newModelFoundRelations)
            {
                string consumer = relations.Key;
                foreach (string provider in relations.Value)
                {
                    Logger.LogUserMessage($" {consumer} -> {provider}");
                }
            }

            Console.WriteLine("");
        }

        private void FindElements()
        {
            foreach (IDsiElement element in _oldModel.GetElements())
            {
                _oldModelFoundElements.Add(element.Name);
                _allElements.Add(element.Name);
            }

            foreach (IDsiElement element in _newModel.GetElements())
            {
                _newModelFoundElements.Add(element.Name);
                _allElements.Add(element.Name);
            }
        }

        private void CompareElement(string elementName)
        {
            if (!_oldModelFoundElements.Contains(elementName))
            {
                _oldModelMissingElements.Add(elementName);
            }

            if (!_newModelFoundElements.Contains(elementName))
            {
                _newModelMissingElements.Add(elementName);
            }
        }

        private void CompareElementRelations(string consumerName)
        {
            _oldModelFoundRelations[consumerName] = new HashSet<string>();
            _newModelFoundRelations[consumerName] = new HashSet<string>();
            _oldModelMissingRelations[consumerName] = new HashSet<string>();
            _newModelMissingRelations[consumerName] = new HashSet<string>();
            _allRelations[consumerName] = new HashSet<string>();

            foreach (IDsiRelation relation in _oldModel.GetRelations())
            {
                IDsiElement consumer = _oldModel.FindElementById(relation.ConsumerId);
                IDsiElement provider = _oldModel.FindElementById(relation.ProviderId);

                if (consumer.Name == consumerName)
                {
                    _oldModelFoundRelations[consumerName].Add(provider.Name);
                    _allRelations[consumerName].Add(provider.Name);
                }
            }

            foreach (IDsiRelation relation in _newModel.GetRelations())
            {
                IDsiElement consumer = _oldModel.FindElementById(relation.ConsumerId);
                IDsiElement provider = _oldModel.FindElementById(relation.ProviderId);

                if ((consumer != null) && (provider != null) && (consumer.Name == consumerName))
                {
                    _newModelFoundRelations[consumerName].Add(provider.Name);
                    _allRelations[consumerName].Add(provider.Name);
                }
            }

            foreach (string providerName in _allRelations[consumerName])
            {
                if (!_oldModelFoundRelations[consumerName].Contains(providerName))
                {
                    _oldModelMissingRelations[consumerName].Add(providerName);
                }

                if (!_newModelFoundRelations[consumerName].Contains(providerName))
                {
                    _newModelMissingRelations[consumerName].Add(providerName);
                }
            }
        }

        private bool AreIdentical()
        {
            bool identical = !(_oldModelMissingElements.Count > 0);

            if (_newModelMissingElements.Count > 0)
            {
                identical = false;
            }

            foreach (string elementName in _allElements)
            {
                if (_oldModelMissingRelations[elementName].Count > 0)
                {
                    identical = false;
                }

                if (_oldModelMissingRelations[elementName].Count > 0)
                {
                    identical = false;
                }
            }
            return identical;
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
